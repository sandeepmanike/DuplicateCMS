using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CollegeManagement.API.Data;
using CollegeManagement.API.DTOs.Timetable;
using CollegeManagement.API.Repositories.Interfaces;
using CollegeManagement.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CollegeManagement.API.Services.Implementations
{
    public class TimetableService : ITimetableService
    {
        private readonly ITimetableRepository _timetableRepository;
        private readonly IPeriodRepository _periodRepository;
        private readonly IRoomRepository _roomRepository;
        private readonly AppDbContext _context;

        public TimetableService(
            ITimetableRepository timetableRepository,
            IPeriodRepository periodRepository,
            IRoomRepository roomRepository,
            AppDbContext context)
        {
            _timetableRepository = timetableRepository;
            _periodRepository = periodRepository;
            _roomRepository = roomRepository;
            _context = context;
        }

        public async Task<TimetableResponseDto?> GetByIdAsync(int id)
        {
            return await _timetableRepository.GetByIdAsync(id);
        }

        public async Task<(IEnumerable<TimetableResponseDto> Items, int TotalCount)> GetPagedAsync(TimetableQueryParams queryParams)
        {
            return await _timetableRepository.GetPagedAsync(queryParams);
        }

        public async Task<IEnumerable<TimetableResponseDto>> GetFacultyTimetableAsync(int facultyId, int? academicYearId = null)
        {
            return await _timetableRepository.GetByFacultyIdAsync(facultyId, academicYearId);
        }

        public async Task<IEnumerable<TimetableResponseDto>> GetSectionTimetableAsync(int sectionId, int? academicYearId = null, bool? isPublished = null)
        {
            return await _timetableRepository.GetBySectionIdAsync(sectionId, academicYearId, isPublished);
        }

        public async Task<IEnumerable<TimetableResponseDto>> GetStudentTimetableAsync(int studentId)
        {
            var student = await _context.Students.FirstOrDefaultAsync(s => s.StudentId == studentId);
            if (student == null)
            {
                throw new KeyNotFoundException($"Student with ID {studentId} not found.");
            }

            var section = await _context.Sections.FirstOrDefaultAsync(sec => sec.SectionName == student.Section || sec.SectionId == student.GroupId);
            if (section == null)
            {
                section = await _context.Sections.FirstOrDefaultAsync();
            }

            int sectionId = section?.SectionId ?? 1;
            return await _timetableRepository.GetBySectionIdAsync(sectionId, student.AcademicYearId, isPublished: true);
        }

        public async Task<IEnumerable<AllocatedFacultyDto>> GetAllocatedFacultiesAsync(int? boardId, int? academicLevelId, int? academicYearId, int? groupId, int? sectionId, int? subjectId)
        {
            return await _timetableRepository.GetAllocatedFacultiesAsync(boardId, academicLevelId, academicYearId, groupId, sectionId, subjectId);
        }

        public async Task<TimetableResponseDto> CreateAsync(CreateTimetableDto dto)
        {
            await ValidateSlotAndConflictsAsync(dto.AcademicYearId, dto.SectionId, dto.FacultyId, dto.RoomId, dto.DayOfWeek, dto.PeriodId, dto.SubjectId, dto.BoardId, dto.GroupId, dto.AcademicLevelId, excludeId: null);

            int newId = await _timetableRepository.AddAsync(dto);
            var result = await _timetableRepository.GetByIdAsync(newId);
            return result ?? throw new InvalidOperationException("Failed to retrieve created timetable slot.");
        }

        public async Task<TimetableResponseDto?> UpdateAsync(int id, UpdateTimetableDto dto)
        {
            var existing = await _timetableRepository.GetByIdAsync(id);
            if (existing == null) return null;

            await ValidateSlotAndConflictsAsync(dto.AcademicYearId, dto.SectionId, dto.FacultyId, dto.RoomId, dto.DayOfWeek, dto.PeriodId, dto.SubjectId, dto.BoardId, dto.GroupId, dto.AcademicLevelId, excludeId: id);

            await _timetableRepository.UpdateAsync(id, dto);
            return await _timetableRepository.GetByIdAsync(id);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _timetableRepository.GetByIdAsync(id);
            if (existing == null) return false;

            await _timetableRepository.DeleteAsync(id);
            return true;
        }

        public async Task<bool> TogglePublishSlotAsync(int id, bool isPublished)
        {
            var existing = await _timetableRepository.GetByIdAsync(id);
            if (existing == null) return false;

            await _timetableRepository.TogglePublishSlotAsync(id, isPublished);
            return true;
        }

        public async Task<bool> PublishSectionTimetableAsync(int sectionId, int academicYearId, bool isPublished)
        {
            await _timetableRepository.PublishSectionTimetableAsync(sectionId, academicYearId, isPublished);
            return true;
        }

        public async Task<bool> CopyTimetableAsync(CopyTimetableDto dto)
        {
            var sourceSlots = await _timetableRepository.GetBySectionIdAsync(dto.SourceSectionId, dto.SourceAcademicYearId);
            if (!sourceSlots.Any())
            {
                throw new InvalidOperationException("No timetable slots found in source section to copy.");
            }

            await _timetableRepository.CopySectionTimetableAsync(dto);
            return true;
        }

        private async Task ValidateSlotAndConflictsAsync(int academicYearId, int sectionId, int facultyId, int roomId, int dayOfWeek, int periodId, int subjectId, int boardId, int groupId, int academicLevelId, int? excludeId)
        {
            // 1. Master IDs existence check
            var board = await _context.Boards.FindAsync(boardId);
            if (board == null) throw new InvalidOperationException($"Board with ID {boardId} does not exist.");

            var academicLevel = await _context.AcademicLevels.FindAsync(academicLevelId);
            if (academicLevel == null) throw new InvalidOperationException($"Academic Level with ID {academicLevelId} does not exist.");

            var academicYear = await _context.AcademicYears.FindAsync(academicYearId);
            if (academicYear == null) throw new InvalidOperationException($"Academic Year with ID {academicYearId} does not exist.");

            var group = await _context.Groups.FindAsync(groupId);
            if (group == null) throw new InvalidOperationException($"Group with ID {groupId} does not exist.");

            var section = await _context.Sections.FindAsync(sectionId);
            if (section == null) throw new InvalidOperationException($"Section with ID {sectionId} does not exist.");

            var subject = await _context.Subjects.FindAsync(subjectId);
            if (subject == null) throw new InvalidOperationException($"Subject with ID {subjectId} does not exist.");

            var faculty = await _context.Faculties.FindAsync(facultyId);
            if (faculty == null || (faculty.IsDeleted)) throw new InvalidOperationException($"Faculty with ID {facultyId} is inactive or does not exist.");

            // 2. Period Break check
            var period = await _periodRepository.GetByIdAsync(periodId);
            if (period == null)
            {
                throw new InvalidOperationException($"Period with ID {periodId} does not exist.");
            }
            if (period.IsBreak)
            {
                throw new InvalidOperationException($"Period '{period.PeriodName}' is designated as a Break period. Classes cannot be scheduled during break times.");
            }

            // 3. Room existence check
            var room = await _roomRepository.GetByIdAsync(roomId);
            if (room == null || !room.IsActive)
            {
                throw new InvalidOperationException($"Room with ID {roomId} is inactive or does not exist.");
            }

            // 4. Faculty Allocation Eligibility check
            var isAllocated = await _context.FacultySubjectAllocations
                .AnyAsync(fsa => fsa.FacultyId == facultyId);
            if (!isAllocated)
            {
                throw new InvalidOperationException($"Faculty '{faculty.FirstName} {faculty.LastName}' is not allocated to teach subject '{subject.SubjectName}'.");
            }

            // 5. Section Slot Conflict check
            bool sectionConflict = await _timetableRepository.HasSectionSlotConflictAsync(academicYearId, sectionId, dayOfWeek, periodId, excludeId);
            if (sectionConflict)
            {
                throw new InvalidOperationException($"Section conflict: This section already has a subject scheduled on Day {dayOfWeek} during Period {periodId}.");
            }

            // 6. Faculty Slot Conflict check
            bool facultyConflict = await _timetableRepository.HasFacultySlotConflictAsync(academicYearId, facultyId, dayOfWeek, periodId, excludeId);
            if (facultyConflict)
            {
                throw new InvalidOperationException($"Faculty conflict: The selected faculty member is already teaching another class on Day {dayOfWeek} during Period {periodId}.");
            }

            // 7. Room Slot Conflict check
            bool roomConflict = await _timetableRepository.HasRoomSlotConflictAsync(academicYearId, roomId, dayOfWeek, periodId, excludeId);
            if (roomConflict)
            {
                throw new InvalidOperationException($"Room conflict: Room '{room.RoomCode}' ({room.RoomName}) is already booked for another section on Day {dayOfWeek} during Period {periodId}.");
            }
        }
    }
}
