using CollegeManagement.API.DTOs.Students;

namespace CollegeManagement.API.Services
{
    public interface IStudentService
    {
        // =========================================================
        // STUDENT CRUD
        // =========================================================

        Task<List<StudentListItemDto>> GetAllAsync();

        Task<StudentResponse?> GetByIdAsync(
            int studentId);
        Task<StudentResponse?> UpdateAsync(
                    int studentId,
                    UpdateStudentRequest request);

        Task<bool> DeleteAsync(
            int studentId);


        // =========================================================
        // STUDENT PROFILE
        // =========================================================

        Task<StudentProfileDto?> GetProfileAsync(
            int studentId);

        Task<StudentProfileDto?> UpdateProfileAsync(
            int studentId,
            UpdateStudentProfileRequest request);


        // =========================================================
        // ACADEMIC OPERATIONS
        // =========================================================

        Task<bool> ChangeSectionAsync(
            int studentId,
            ChangeSectionRequest request);

        Task<bool> ChangeGroupAsync(
            int studentId,
            ChangeGroupRequest request);

        Task<bool> TransferAsync(
            int studentId,
            TransferStudentRequest request);


        // =========================================================
        // STUDENT STATUS
        // =========================================================

        Task<bool> SuspendAsync(
            int studentId,
            SuspendStudentRequest request);

        Task<bool> ActivateAsync(
            int studentId);


        // =========================================================
        // AUTHENTICATION
        // =========================================================

        Task<bool> ResetPasswordAsync(
            int studentId);


        // =========================================================
        // DASHBOARD
        // =========================================================

        Task<StudentDashboardDto?> GetDashboardAsync(
            int studentId);


        // =========================================================
        // SEARCH / FILTER
        // =========================================================

        Task<List<StudentListItemDto>> SearchAsync(
            string? search,
            int? groupId,
            int? sectionId,
            int? academicYearId,
            bool? isActive);

        Task<List<StudentListItemDto>> GetByGroupAsync(
            int groupId);

        Task<List<StudentListItemDto>> GetBySectionAsync(
            int sectionId);

        Task<List<StudentListItemDto>> GetActiveAsync();


        // =========================================================
        // DUPLICATE VALIDATION
        // =========================================================

        Task<bool> EmailExistsAsync(
            string email,
            int? excludeStudentId = null);

        Task<bool> MobileExistsAsync(
            string mobile,
            int? excludeStudentId = null);
    }
}