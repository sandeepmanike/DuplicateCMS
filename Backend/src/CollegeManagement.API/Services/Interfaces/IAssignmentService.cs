using CollegeManagement.API.DTOs.Assignment;

namespace CollegeManagement.API.Services.Interfaces
{
    public interface IAssignmentService
    {
        Task<IEnumerable<AssignmentResponseDto>> GetAllAsync();

        Task<AssignmentResponseDto?> GetByIdAsync(int id);

        Task<AssignmentResponseDto> CreateAsync(CreateAssignmentDto dto);

        Task<AssignmentResponseDto?> UpdateAsync(int id, UpdateAssignmentDto dto);

        Task<bool> DeleteAsync(int id);

        Task<bool> SubmitAssignmentAsync(int assignmentId, SubmitAssignmentDto dto);

        Task<IEnumerable<AssignmentSubmissionResponseDto>> GetSubmissionsAsync(int assignmentId);
    }
}