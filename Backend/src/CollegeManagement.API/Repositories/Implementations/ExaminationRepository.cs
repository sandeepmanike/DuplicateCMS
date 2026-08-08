using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CollegeManagement.API.Data;
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
                .Include(e => e.AssessmentType)
                .FirstOrDefaultAsync(e => e.ExaminationId == examinationId);
        }

        public async Task<IEnumerable<Examination>> GetExaminationsAsync(string? courseId)
        {
            var query = _context.Examinations
                .Include(e => e.Board)
                .Include(e => e.AcademicYear)
                .Include(e => e.AcademicLevel)
                .Include(e => e.Group)
                .Include(e => e.AssessmentType)
                .Where(e => e.IsActive)
                .AsQueryable();

            if (!string.IsNullOrEmpty(courseId) && int.TryParse(courseId, out int groupFilterId))
            {
                query = query.Where(e => e.GroupId == groupFilterId);
            }

            return await query.ToListAsync();
        }

        public async Task UpdateExaminationAsync(Examination examination)
        {
            _context.Examinations.Update(examination);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteExaminationAsync(Examination examination)
        {
            // Perform soft delete
            examination.IsActive = false;
            _context.Examinations.Update(examination);
            return await _context.SaveChangesAsync() > 0;
        }

        #endregion

        #region Exam Schedule Methods

        public async Task<ExamSchedule> CreateExamScheduleAsync(ExamSchedule schedule)
        {
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

            return await query.ToListAsync();
        }

        public async Task UpdateExamScheduleAsync(ExamSchedule schedule)
        {
            _context.ExamSchedules.Update(schedule);
            await _context.SaveChangesAsync();
        }

        public async Task<int> PublishExamSchedulesAsync(IEnumerable<int> scheduleIds)
        {
            var schedules = await _context.ExamSchedules
                .Where(s => scheduleIds.Contains(s.ExamScheduleId))
                .ToListAsync();

            foreach (var schedule in schedules)
            {
                schedule.IsActive = true;
            }

            return await _context.SaveChangesAsync();
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