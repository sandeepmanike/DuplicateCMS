using CollegeManagement.API.DTOs.Students;
using CollegeManagement.API.Repositories;

namespace CollegeManagement.API.Services
{
    public class StudentService : IStudentService
    {
        private readonly IStudentRepository _repository;

        public StudentService(IStudentRepository repository)
        {
            _repository = repository;
        }

        // =========================================================
        // STUDENT CRUD
        // =========================================================

        public async Task<List<StudentListItemDto>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<StudentResponse?> GetByIdAsync(int studentId)
        {
            return await _repository.GetByIdAsync(studentId);
        }

        public async Task<StudentResponse> CreateAsync(
            CreateStudentRequest request)
        {
            return await _repository.CreateAsync(request);
        }

        public async Task<StudentResponse?> UpdateAsync(
            int studentId,
            UpdateStudentRequest request)
        {
            return await _repository.UpdateAsync(studentId, request);
        }

        public async Task<bool> DeleteAsync(int studentId)
        {
            return await _repository.DeleteAsync(studentId);
        }


        // =========================================================
        // STUDENT PROFILE
        // =========================================================

        public async Task<StudentProfileDto?> GetProfileAsync(
            int studentId)
        {
            return await _repository.GetProfileAsync(studentId);
        }

        public async Task<StudentProfileDto?> UpdateProfileAsync(
            int studentId,
            StudentProfileDto request)
        {
            return await _repository.UpdateProfileAsync(
                studentId,
                request);
        }


        // =========================================================
        // ACADEMIC OPERATIONS
        // =========================================================

        public async Task<bool> ChangeSectionAsync(
            int studentId,
            ChangeSectionRequest request)
        {
            return await _repository.ChangeSectionAsync(
                studentId,
                request);
        }

        public async Task<bool> ChangeGroupAsync(
            int studentId,
            ChangeGroupRequest request)
        {
            return await _repository.ChangeGroupAsync(
                studentId,
                request);
        }

        public async Task<bool> TransferAsync(
            int studentId,
            TransferStudentRequest request)
        {
            return await _repository.TransferAsync(
                studentId,
                request);
        }


        // =========================================================
        // STUDENT STATUS
        // =========================================================

        public async Task<bool> SuspendAsync(
            int studentId,
            SuspendStudentRequest request)
        {
            return await _repository.SuspendAsync(
                studentId,
                request);
        }

        public async Task<bool> ActivateAsync(
            int studentId)
        {
            return await _repository.ActivateAsync(studentId);
        }


        // =========================================================
        // AUTHENTICATION
        // =========================================================

        public async Task<bool> ResetPasswordAsync(
            int studentId)
        {
            return await _repository.ResetPasswordAsync(studentId);
        }


        // =========================================================
        // DASHBOARD
        // =========================================================

        public async Task<StudentDashboardDto?> GetDashboardAsync(
            int studentId)
        {
            return await _repository.GetDashboardAsync(studentId);
        }


        // =========================================================
        // SEARCH STUDENTS
        // =========================================================

        public async Task<List<StudentListItemDto>> SearchAsync(
            string? search,
            int? boardId,
            int? academicYearId,
            int? academicLevelId,
            int? groupId,
            int? sectionId,
            bool? isActive)
        {
            return await _repository.SearchAsync(
                search,
                boardId,
                academicYearId,
                academicLevelId,
                groupId,
                sectionId,
                isActive);
        }


        // =========================================================
        // GET STUDENTS BY GROUP
        // =========================================================

        public async Task<List<StudentListItemDto>> GetByGroupAsync(
            int groupId)
        {
            return await _repository.GetByGroupAsync(groupId);
        }


        // =========================================================
        // GET STUDENTS BY SECTION
        // =========================================================

        public async Task<List<StudentListItemDto>> GetBySectionAsync(
            int sectionId)
        {
            return await _repository.GetBySectionAsync(sectionId);
        }


        // =========================================================
        // GET ACTIVE STUDENTS
        // =========================================================

        public async Task<List<StudentListItemDto>> GetActiveAsync()
        {
            return await _repository.GetActiveAsync();
        }


        // =========================================================
        // CHECK EMAIL EXISTS
        // =========================================================

        public async Task<bool> EmailExistsAsync(
            string email,
            int? excludeStudentId = null)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            return await _repository.EmailExistsAsync(
                email,
                excludeStudentId);
        }


        // =========================================================
        // CHECK MOBILE EXISTS
        // =========================================================

        public async Task<bool> MobileExistsAsync(
            string mobile,
            int? excludeStudentId = null)
        {
            if (string.IsNullOrWhiteSpace(mobile))
                return false;

            return await _repository.MobileExistsAsync(
                mobile,
                excludeStudentId);
        }
    }
}