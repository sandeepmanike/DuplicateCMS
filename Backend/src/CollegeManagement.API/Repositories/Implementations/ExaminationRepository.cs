using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CollegeManagement.API.Data;
using CollegeManagement.API.DTOs.Examination.Requests;
using CollegeManagement.API.Models;
using CollegeManagement.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CollegeManagement.API.Repositories.Implementations
{
    public class ExaminationRepository : IExaminationRepository
    {
        private readonly AppDbContext _context;

        public ExaminationRepository(AppDbContext context)
        {
            _context = context;
        }

        #region Examination Methods

        public async Task<Examination> CreateExaminationAsync(Examination examination)
        {
            _context.Examinations.Add(examination);
            await _context.SaveChangesAsync();
            return examination;
        }

        public async Task<Examination?> GetExaminationByIdAsync(int examinationId)
        {
            return await _context.Examinations
                .Include(e => e.Board)
                .Include(e => e.AcademicYear)
                .Include(e => e.AcademicLevel)
                .Include(e => e.Group)
                .Include(e => e.Program)
                .Include(e => e.AssessmentType)
                .Include(e => e.ExamSchedules.Where(s => s.IsActive))
                    .ThenInclude(s => s.Subject)
                .FirstOrDefaultAsync(e => e.ExaminationId == examinationId);
        }

        public async Task<IEnumerable<Examination>> GetExaminationsAsync(ExaminationSearchRequestDto filter)
        {
            var query = _context.Examinations
                .Include(e => e.Board)
                .Include(e => e.AcademicYear)
                .Include(e => e.AcademicLevel)
                .Include(e => e.Group)
                .Include(e => e.Program)
                .Include(e => e.AssessmentType)
                .Include(e => e.ExamSchedules.Where(s => s.IsActive))
                    .ThenInclude(s => s.Subject)
                .Where(e => e.IsActive)
                .AsQueryable();

            if (filter.BoardId.HasValue && filter.BoardId.Value > 0)
                query = query.Where(e => e.BoardId == filter.BoardId.Value);

            if (filter.AcademicYearId.HasValue && filter.AcademicYearId.Value > 0)
                query = query.Where(e => e.AcademicYearId == filter.AcademicYearId.Value);

            if (filter.AcademicLevelId.HasValue && filter.AcademicLevelId.Value > 0)
                query = query.Where(e => e.AcademicLevelId == filter.AcademicLevelId.Value);

            if (filter.GroupId.HasValue && filter.GroupId.Value > 0)
                query = query.Where(e => e.GroupId == filter.GroupId.Value);

            if (filter.ProgramId.HasValue && filter.ProgramId.Value > 0)
                query = query.Where(e => e.ProgramId == filter.ProgramId.Value);

            if (filter.AssessmentTypeId.HasValue && filter.AssessmentTypeId.Value > 0)
                query = query.Where(e => e.AssessmentTypeId == filter.AssessmentTypeId.Value);

            if (!string.IsNullOrWhiteSpace(filter.ExamType))
                query = query.Where(e => e.AssessmentType != null && e.AssessmentType.AssessmentTypeName.ToLower().Contains(filter.ExamType.ToLower()));

            if (!string.IsNullOrWhiteSpace(filter.Status))
                query = query.Where(e => e.Status.ToLower() == filter.Status.ToLower());

            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                var search = filter.SearchTerm.Trim().ToLower();
                query = query.Where(e =>
                    e.ExamName.ToLower().Contains(search) ||
                    (e.ExamCode != null && e.ExamCode.ToLower().Contains(search)) ||
                    (e.Group != null && e.Group.GroupName.ToLower().Contains(search)) ||
                    (e.Program != null && e.Program.ProgramName.ToLower().Contains(search)));
            }

            return await query.OrderByDescending(e => e.ExaminationId).ToListAsync();
        }

        public async Task UpdateExaminationAsync(Examination examination)
        {
            examination.UpdatedAt = DateTime.UtcNow;
            _context.Examinations.Update(examination);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteExaminationAsync(Examination examination)
        {
            // Perform soft delete
            examination.IsActive = false;
            examination.UpdatedAt = DateTime.UtcNow;
            _context.Examinations.Update(examination);
            return await _context.SaveChangesAsync() > 0;
        }

        #endregion

        #region Exam Schedule Methods

        public async Task<ExamSchedule> CreateExamScheduleAsync(ExamSchedule schedule)
        {
            schedule.CreatedAt = DateTime.UtcNow;
            _context.ExamSchedules.Add(schedule);
            await _context.SaveChangesAsync();
            return schedule;
        }

        public async Task<ExamSchedule?> GetExamScheduleByIdAsync(int examScheduleId)
        {
            return await _context.ExamSchedules
                .Include(s => s.Examination)
                .Include(s => s.Subject)
                .FirstOrDefaultAsync(s => s.ExamScheduleId == examScheduleId);
        }

        public async Task<IEnumerable<ExamSchedule>> GetExamSchedulesAsync(int? examinationId)
        {
            var query = _context.ExamSchedules
                .Include(s => s.Examination)
                .Include(s => s.Subject)
                .Where(s => s.IsActive)
                .AsQueryable();

            if (examinationId.HasValue)
            {
                query = query.Where(s => s.ExaminationId == examinationId.Value);
            }

            return await query.OrderBy(s => s.ExamDate).ThenBy(s => s.StartTime).ToListAsync();
        }

        public async Task UpdateExamScheduleAsync(ExamSchedule schedule)
        {
            schedule.UpdatedAt = DateTime.UtcNow;
            _context.ExamSchedules.Update(schedule);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteExamScheduleAsync(ExamSchedule schedule)
        {
            schedule.IsActive = false;
            schedule.UpdatedAt = DateTime.UtcNow;
            _context.ExamSchedules.Update(schedule);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<int> PublishExamSchedulesAsync(IEnumerable<int> scheduleIds)
        {
            var schedules = await _context.ExamSchedules
                .Where(s => scheduleIds.Contains(s.ExamScheduleId))
                .ToListAsync();

            foreach (var schedule in schedules)
            {
                schedule.IsActive = true;
                schedule.UpdatedAt = DateTime.UtcNow;
            }

            return await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Subject>> GetEligibleSubjectsForExamAsync(int examinationId)
        {
            var exam = await _context.Examinations.FirstOrDefaultAsync(e => e.ExaminationId == examinationId);
            if (exam == null) return Enumerable.Empty<Subject>();

            return await _context.Subjects
                .Where(s => s.IsActive
                    && s.BoardId == exam.BoardId
                    && s.AcademicLevelId == exam.AcademicLevelId
                    && s.GroupId == exam.GroupId)
                .OrderBy(s => s.SubjectName)
                .ToListAsync();
        }

        public async Task<bool> HasRoomConflictAsync(DateOnly examDate, TimeOnly startTime, TimeOnly endTime, string hall, int? excludeScheduleId = null)
        {
            if (string.IsNullOrWhiteSpace(hall)) return false;

            return await _context.ExamSchedules
                .AnyAsync(s => s.IsActive
                    && s.ExamDate == examDate
                    && s.Hall.ToLower() == hall.Trim().ToLower()
                    && (!excludeScheduleId.HasValue || s.ExamScheduleId != excludeScheduleId.Value)
                    && !(endTime <= s.StartTime || startTime >= s.EndTime));
        }

        public async Task<bool> HasInvigilatorConflictAsync(DateOnly examDate, TimeOnly startTime, TimeOnly endTime, string invigilator, int? excludeScheduleId = null)
        {
            if (string.IsNullOrWhiteSpace(invigilator)) return false;

            return await _context.ExamSchedules
                .AnyAsync(s => s.IsActive
                    && s.ExamDate == examDate
                    && s.Invigilator.ToLower() == invigilator.Trim().ToLower()
                    && (!excludeScheduleId.HasValue || s.ExamScheduleId != excludeScheduleId.Value)
                    && !(endTime <= s.StartTime || startTime >= s.EndTime));
        }

        public async Task<IEnumerable<Models.Timetable.Room>> GetAvailableHallsAsync(DateOnly examDate, TimeOnly startTime, TimeOnly endTime, int? excludeScheduleId = null)
        {
            var bookedHalls = await _context.ExamSchedules
                .Where(s => s.IsActive
                    && s.ExamDate == examDate
                    && (!excludeScheduleId.HasValue || s.ExamScheduleId != excludeScheduleId.Value)
                    && !(endTime <= s.StartTime || startTime >= s.EndTime))
                .Select(s => s.Hall.Trim().ToLower())
                .Distinct()
                .ToListAsync();

            var bookedRoomIds = await _context.ExamSchedules
                .Where(s => s.IsActive
                    && s.ExamDate == examDate
                    && s.RoomId.HasValue
                    && (!excludeScheduleId.HasValue || s.ExamScheduleId != excludeScheduleId.Value)
                    && !(endTime <= s.StartTime || startTime >= s.EndTime))
                .Select(s => s.RoomId!.Value)
                .Distinct()
                .ToListAsync();

            var allRooms = await _context.Rooms
                .Where(r => r.IsActive)
                .OrderBy(r => r.RoomNumber)
                .ToListAsync();

            return allRooms.Where(r => 
                !bookedRoomIds.Contains(r.RoomId) &&
                !bookedHalls.Contains(r.RoomNumber.Trim().ToLower()) &&
                (string.IsNullOrWhiteSpace(r.RoomName) || !bookedHalls.Contains(r.RoomName.Trim().ToLower())));
        }

        public async Task<IEnumerable<Models.Faculty.Faculty>> GetAvailableInvigilatorsAsync(DateOnly examDate, TimeOnly startTime, TimeOnly endTime, int? excludeScheduleId = null)
        {
            var bookedInvigilatorNames = await _context.ExamSchedules
                .Where(s => s.IsActive
                    && s.ExamDate == examDate
                    && (!excludeScheduleId.HasValue || s.ExamScheduleId != excludeScheduleId.Value)
                    && !(endTime <= s.StartTime || startTime >= s.EndTime))
                .Select(s => s.Invigilator.Trim().ToLower())
                .Distinct()
                .ToListAsync();

            var bookedInvigilatorIds = await _context.ExamSchedules
                .Where(s => s.IsActive
                    && s.ExamDate == examDate
                    && s.InvigilatorId.HasValue
                    && (!excludeScheduleId.HasValue || s.ExamScheduleId != excludeScheduleId.Value)
                    && !(endTime <= s.StartTime || startTime >= s.EndTime))
                .Select(s => s.InvigilatorId!.Value)
                .Distinct()
                .ToListAsync();

            var allFaculty = await _context.Faculties
                .Include(f => f.DesignationRef)
                .Where(f => !f.IsDeleted && f.Status == "Active")
                .OrderBy(f => f.FirstName)
                .ThenBy(f => f.LastName)
                .ToListAsync();

            return allFaculty.Where(f =>
                !bookedInvigilatorIds.Contains(f.Id) &&
                !bookedInvigilatorNames.Contains($"{f.FirstName} {f.LastName}".Trim().ToLower()) &&
                !bookedInvigilatorNames.Contains(f.FirstName.Trim().ToLower()));
        }

        #endregion

        #region Hall Ticket Methods
        public async Task<IEnumerable<HallTicket>> GenerateHallTicketsAsync(int examinationId, int batchId)
        {
            var existingTickets = await _context.HallTickets
                .Where(h => h.ExaminationId == examinationId && h.BatchId == batchId)
                .ToListAsync();

            if (existingTickets.Any())
            {
                return existingTickets;
            }

            var users = await _context.Users.ToListAsync();

            var newTickets = users.Select(u => new HallTicket
            {
                ExaminationId = examinationId,
                StudentId = u.UserId,
                BatchId = batchId,
                GeneratedAt = DateTime.UtcNow
            }).ToList();

            _context.HallTickets.AddRange(newTickets);
            await _context.SaveChangesAsync();

            return newTickets;
        }





        public async Task<Stream?> GetHallTicketPdfStreamAsync(int studentId, int examinationId)
        {
            var ticket = await _context.HallTickets
                .FirstOrDefaultAsync(h => h.StudentId == studentId && h.ExaminationId == examinationId);

            if (ticket == null) return null;

            return new MemoryStream();
        }

        #endregion

        #region Invigilator Methods

        public async Task AssignInvigilatorsAsync(int examScheduleId, IEnumerable<int> invigilatorIds, string hallNumber)
        {
            var assignments = invigilatorIds.Select(id => new InvigilatorAssignment
            {
                ExamScheduleId = examScheduleId,
                InvigilatorId = id,
                HallNumber = hallNumber,
                AssignedAt = DateTime.UtcNow
            });

            _context.InvigilatorAssignments.AddRange(assignments);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<InvigilatorAssignment>> GetInvigilatorsByScheduleIdAsync(int examScheduleId)
        {
            return await _context.InvigilatorAssignments
                .Include(i => i.Invigilator)
                .Where(i => i.ExamScheduleId == examScheduleId)
                .ToListAsync();
        }

        #endregion
    }
}