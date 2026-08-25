using CollegeManagement.API.DTOs.Students;

namespace CollegeManagement.API.Services
{
    public interface IStudentService
    {
        Task<List<StudentListItemDto>> GetAllAsync();
        Task<StudentResponse?> GetByIdAsync(int studentId);
        Task<StudentResponse?> UpdateAsync(int studentId, UpdateStudentRequest request);
        Task<bool> DeleteAsync(int studentId);
        Task<StudentProfileDto?> GetProfileAsync(int studentId);
        Task<StudentProfileDto?> UpdateProfileAsync(int studentId, UpdateStudentProfileRequest request);
        Task<bool> ChangeSectionAsync(int studentId, ChangeSectionRequest request);
        Task<bool> ChangeGroupAsync(int studentId, ChangeGroupRequest request);
        Task<bool> TransferAsync(int studentId, TransferStudentRequest request);
        Task<bool> SuspendAsync(int studentId, SuspendStudentRequest request);
        Task<bool> ActivateAsync(int studentId);
        Task<bool> ResetPasswordAsync(int studentId);
        Task<StudentDashboardDto?> GetDashboardAsync(int studentId);
        Task<List<StudentListItemDto>> SearchAsync(
            string? search,
            int? boardId,
            int? academicYearId,
            int? academicLevelId,
            int? groupId,
            int? sectionId,
            bool? isActive);
        Task<List<StudentListItemDto>> GetByGroupAsync(int groupId);
        Task<List<StudentListItemDto>> GetBySectionAsync(int sectionId);
        Task<List<StudentListItemDto>> GetActiveAsync();
        Task<bool> EmailExistsAsync(string email, int? excludeStudentId = null);
        Task<bool> MobileExistsAsync(string mobile, int? excludeStudentId = null);
    }
}
