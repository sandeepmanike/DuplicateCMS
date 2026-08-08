using CollegeManagement.API.DTOs.Students;

namespace CollegeManagement.API.Repositories
{
    public interface IStudentRepository
    {
        // ==========================
        // Student CRUD
        // ==========================

        Task<List<StudentListItemDto>> GetAllAsync();

        Task<StudentResponse?> GetByIdAsync(int studentId);

        Task<StudentResponse> CreateAsync(CreateStudentRequest request);

        Task<StudentResponse?> UpdateAsync(
            int studentId,
            UpdateStudentRequest request);

        Task<bool> DeleteAsync(int studentId);

        // ==========================
        // Student Profile
        // ==========================

        Task<StudentProfileDto?> GetProfileAsync(int studentId);

        Task<StudentProfileDto?> UpdateProfileAsync(
            int studentId,
            StudentProfileDto request);

        // ==========================
        // Student Academic
        // ==========================

        Task<bool> ChangeSectionAsync(
            int studentId,
            ChangeSectionRequest request);

        Task<bool> ChangeGroupAsync(
            int studentId,
            ChangeGroupRequest request);

        Task<bool> TransferAsync(
            int studentId,
            TransferStudentRequest request);

        // ==========================
        // Student Status
        // ==========================

        Task<bool> SuspendAsync(
            int studentId,
            SuspendStudentRequest request);

        Task<bool> ActivateAsync(int studentId);

        // ==========================
        // Authentication
        // ==========================

        Task<bool> ResetPasswordAsync(int studentId);

        // ==========================
        // Dashboard
        // ==========================

        Task<StudentDashboardDto?> GetDashboardAsync(int studentId);
    }
}