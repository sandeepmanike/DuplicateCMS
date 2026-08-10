using CollegeManagement.API.DTOs.Assignment;
using CollegeManagement.API.DTOs.Faculty;

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

        Task<IEnumerable<SubjectDropdownDto>>
GetSubjectsByGroupAsync(int groupId);

        Task<IEnumerable<FacultyDropdownDto>>
        GetFacultyDropdownAsync(
            int subjectId,
            int groupId,
            int academicYearId,
            string academicLevel);
    }
}