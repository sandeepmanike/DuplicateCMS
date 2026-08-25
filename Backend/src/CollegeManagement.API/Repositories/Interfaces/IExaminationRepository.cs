using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using CollegeManagement.API.DTOs.Examination.Requests;
using CollegeManagement.API.Models;

namespace CollegeManagement.API.Repositories.Interfaces
{
    public interface IExaminationRepository
    {
        Task<Examination> CreateExaminationAsync(Examination examination);
        Task<Examination?> GetExaminationByIdAsync(int examinationId);
        Task<IEnumerable<Examination>> GetExaminationsAsync(ExaminationSearchRequestDto filter);
        Task UpdateExaminationAsync(Examination examination);
        Task<bool> DeleteExaminationAsync(Examination examination);

        Task<ExamSchedule> CreateExamScheduleAsync(ExamSchedule schedule);
        Task<ExamSchedule?> GetExamScheduleByIdAsync(int examScheduleId);
        Task<IEnumerable<ExamSchedule>> GetExamSchedulesAsync(int? examinationId);
        Task UpdateExamScheduleAsync(ExamSchedule schedule);
        Task<bool> DeleteExamScheduleAsync(ExamSchedule schedule);
        Task<int> PublishExamSchedulesAsync(IEnumerable<int> scheduleIds);

        Task<IEnumerable<Subject>> GetEligibleSubjectsForExamAsync(int examinationId);
        Task<bool> HasRoomConflictAsync(DateOnly examDate, TimeOnly startTime, TimeOnly endTime, string hall, int? excludeScheduleId = null);
        Task<bool> HasInvigilatorConflictAsync(DateOnly examDate, TimeOnly startTime, TimeOnly endTime, string invigilator, int? excludeScheduleId = null);
        Task<IEnumerable<Models.Timetable.Room>> GetAvailableHallsAsync(DateOnly examDate, TimeOnly startTime, TimeOnly endTime, int? excludeScheduleId = null);
        Task<IEnumerable<Models.Faculty.Faculty>> GetAvailableInvigilatorsAsync(DateOnly examDate, TimeOnly startTime, TimeOnly endTime, int? excludeScheduleId = null);

        Task<IEnumerable<HallTicket>> GenerateHallTicketsAsync(int examinationId, int batchId);
        Task<Stream?> GetHallTicketPdfStreamAsync(int studentId, int examinationId);

        Task AssignInvigilatorsAsync(int examScheduleId, IEnumerable<int> invigilatorIds, string hallNumber);
        Task<IEnumerable<InvigilatorAssignment>> GetInvigilatorsByScheduleIdAsync(int examScheduleId);
    }
}