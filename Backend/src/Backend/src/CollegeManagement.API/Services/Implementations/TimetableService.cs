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
            if (dto.BoardId <= 0) dto.BoardId = (await _context.Boards.FirstOrDefaultAsync(b => b.IsActive))?.BoardId ?? 1;
            if (dto.AcademicLevelId <= 0) dto.AcademicLevelId = (await _context.AcademicLevels.FirstOrDefaultAsync(al => al.IsActive))?.AcademicLevelId ?? 1;
            if (dto.AcademicYearId <= 0) dto.AcademicYearId = (await _context.AcademicYears.FirstOrDefaultAsync(ay => ay.IsActive))?.AcademicYearId ?? 1;
            if (dto.GroupId <= 0) dto.GroupId = (await _context.Groups.FirstOrDefaultAsync(g => g.IsActive))?.GroupId ?? 1;
            if (dto.SectionId <= 0) dto.SectionId = (await _context.Sections.FirstOrDefaultAsync(s => s.IsActive))?.SectionId ?? 1;
            if (dto.SubjectId <= 0) dto.SubjectId = (await _context.Subjects.FirstOrDefaultAsync())?.SubjectId ?? 1;
            if (dto.FacultyId <= 0) dto.FacultyId = (await _context.Faculties.FirstOrDefaultAsync(f => !f.IsDeleted))?.Id ?? 1;
            if (dto.RoomId <= 0) dto.RoomId = (await _roomRepository.GetAllAsync())?.FirstOrDefault(r => r.IsActive)?.RoomId ?? 1;
            if (dto.PeriodId <= 0) dto.PeriodId = (await _periodRepository.GetAllAsync())?.FirstOrDefault(p => !p.IsBreak)?.PeriodId ?? 1;
            if (dto.DayOfWeek <= 0) dto.DayOfWeek = 1;

            await ValidateSlotAndConflictsAsync(dto.AcademicYearId, dto.SectionId, dto.FacultyId, dto.RoomId, dto.DayOfWeek, dto.PeriodId, dto.SubjectId, dto.BoardId, dto.GroupId, dto.AcademicLevelId, excludeId: null);

            int newId = await _timetableRepository.AddAsync(dto);
            var result = await _timetableRepository.GetByIdAsync(newId);
            return result ?? throw new InvalidOperationException("Failed to retrieve created timetable slot.");
        }

        public async Task<TimetableResponseDto?> UpdateAsync(int id, UpdateTimetableDto dto)
        {
            var existing = await _timetableRepository.GetByIdAsync(id);
            if (existing == null) return null;

            if (dto.BoardId <= 0) dto.BoardId = existing.BoardId;
            if (dto.AcademicLevelId <= 0) dto.AcademicLevelId = existing.AcademicLevelId;
            if (dto.AcademicYearId <= 0) dto.AcademicYearId = existing.AcademicYearId;
            if (dto.GroupId <= 0) dto.GroupId = existing.GroupId;
            if (dto.SectionId <= 0) dto.SectionId = existing.SectionId;
            if (dto.SubjectId <= 0) dto.SubjectId = existing.SubjectId;
            if (dto.FacultyId <= 0) dto.FacultyId = existing.FacultyId;
            if (dto.RoomId <= 0) dto.RoomId = existing.RoomId;
            if (dto.PeriodId <= 0) dto.PeriodId = existing.PeriodId;
            if (dto.DayOfWeek <= 0) dto.DayOfWeek = existing.DayOfWeek > 0 ? existing.DayOfWeek : 1;

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
            // 1. Master IDs existence check with fallback resolution for 0 IDs
            var board = await _context.Boards.FindAsync(boardId) 
                        ?? await _context.Boards.FirstOrDefaultAsync(b => b.IsActive);
            if (board == null) throw new InvalidOperationException("Please select a valid Board.");

            var academicLevel = await _context.AcademicLevels.FindAsync(academicLevelId) 
                                ?? await _context.AcademicLevels.FirstOrDefaultAsync(al => al.IsActive);
            if (academicLevel == null) throw new InvalidOperationException("Please select a valid Academic Level.");

            var academicYear = await _context.AcademicYears.FindAsync(academicYearId) 
                               ?? await _context.AcademicYears.FirstOrDefaultAsync(ay => ay.IsActive);
            if (academicYear == null) throw new InvalidOperationException("Please select a valid Academic Year.");

            var group = await _context.Groups.FindAsync(groupId) 
                        ?? await _context.Groups.FirstOrDefaultAsync(g => g.IsActive);
            if (group == null) throw new InvalidOperationException("Please select a valid Group.");

            var section = await _context.Sections.FindAsync(sectionId)
                          ?? await _context.Sections.FirstOrDefaultAsync(s => s.IsActive);
            if (section == null) throw new InvalidOperationException("Please select a valid Section.");

            var subject = await _context.Subjects.FindAsync(subjectId)
                         ?? await _context.Subjects.FirstOrDefaultAsync();
            if (subject == null) throw new InvalidOperationException("Please select a valid Subject.");

            var faculty = await _context.Faculties.FindAsync(facultyId)
                          ?? await _context.Faculties.FirstOrDefaultAsync(f => !f.IsDeleted);
            if (faculty == null || faculty.IsDeleted) throw new InvalidOperationException("Please select a valid Faculty.");

            // 2. Period Break check
            var period = await _periodRepository.GetByIdAsync(periodId);
            if (period == null)
            {
                var allPeriods = await _periodRepository.GetAllAsync();
                period = allPeriods.FirstOrDefault(p => !p.IsBreak);
                if (period == null) throw new InvalidOperationException("Please select a valid Period.");
            }
            if (period.IsBreak)
            {
                throw new InvalidOperationException($"Period '{period.PeriodName}' is designated as a Break period. Classes cannot be scheduled during break times.");
            }

            // 3. Room existence check
            var room = await _roomRepository.GetByIdAsync(roomId);
            if (room == null || !room.IsActive)
            {
                var allRooms = await _roomRepository.GetAllAsync();
                room = allRooms.FirstOrDefault(r => r.IsActive);
                if (room == null) throw new InvalidOperationException("Please select a valid Room.");
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
