using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using CollegeManagement.API.DTOs.Examination.Requests;
using CollegeManagement.API.DTOs.Examination.Responses;

namespace CollegeManagement.API.Services.Interfaces
{
    public interface IExaminationService
    {
        // Examination Methods
        Task<ExaminationResponse> CreateExaminationAsync(CreateExaminationRequest request);
        Task<ExaminationResponse?> GetExaminationByIdAsync(int examinationId);
        Task<IEnumerable<ExaminationResponse>> GetExaminationsAsync(string? courseId);
        Task<ExaminationResponse?> UpdateExaminationAsync(int examinationId, UpdateExaminationRequest request);
        Task<bool> DeleteExaminationAsync(int examinationId);
        Task<ExaminationStatusResponse?> CancelExaminationAsync(int examinationId, CancelExaminationRequest request);
        Task<ExaminationStatusResponse?> RescheduleExaminationAsync(int examinationId, RescheduleExaminationRequest request);

        // Exam Schedule Methods
        Task<ExamScheduleResponse> CreateExamScheduleAsync(CreateExamScheduleRequest request);
        Task<ExamScheduleResponse?> GetExamScheduleByIdAsync(int examScheduleId);
        Task<IEnumerable<ExamScheduleResponse>> GetExamSchedulesAsync(int? examinationId);
        Task<ExamScheduleResponse?> UpdateExamScheduleAsync(int examScheduleId, UpdateExamScheduleRequest request);
        Task<int> PublishExamSchedulesAsync(PublishExamScheduleRequest request);

        // Hall Ticket Methods
        Task<IEnumerable<HallTicketResponse>> GenerateHallTicketsAsync(GenerateHallTicketRequest request);
        Task<Stream?> DownloadHallTicketPdfAsync(int studentId, int examinationId);

        // Invigilator Methods
        Task AssignInvigilatorsAsync(AssignInvigilatorRequest request);
        Task<IEnumerable<InvigilatorAssignmentResponse>> GetInvigilatorsAsync(int examScheduleId);
    }
}