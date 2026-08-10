using CollegeManagement.API.Models;

namespace CollegeManagement.API.Repositories.Interfaces
{
    public interface IExaminationRepository
    {
        Task<Examination> CreateExaminationAsync(Examination examination);
        Task<Examination?> GetExaminationByIdAsync(int examinationId);
        Task<IEnumerable<Examination>> GetExaminationsAsync(string? courseId); // Match string? parameter
        Task UpdateExaminationAsync(Examination examination);
        Task<bool> DeleteExaminationAsync(Examination examination);

        Task<ExamSchedule> CreateExamScheduleAsync(ExamSchedule schedule);
        Task<ExamSchedule?> GetExamScheduleByIdAsync(int examScheduleId);
        Task<IEnumerable<ExamSchedule>> GetExamSchedulesAsync(int? examinationId);
        Task UpdateExamScheduleAsync(ExamSchedule schedule);
        Task<int> PublishExamSchedulesAsync(IEnumerable<int> scheduleIds);

        Task<IEnumerable<HallTicket>> GenerateHallTicketsAsync(int examinationId, int batchId);
        Task<Stream?> GetHallTicketPdfStreamAsync(int studentId, int examinationId);

        Task AssignInvigilatorsAsync(int examScheduleId, IEnumerable<int> invigilatorIds, string hallNumber);
        Task<IEnumerable<InvigilatorAssignment>> GetInvigilatorsByScheduleIdAsync(int examScheduleId);
    }
}