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

        // =====================================
        // GET ALL STUDENTS
        // =====================================

        public async Task<List<StudentListItemDto>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        // =====================================
        // GET STUDENT BY ID
        // =====================================

        public async Task<StudentResponse?> GetByIdAsync(int studentId)
        {
            return await _repository.GetByIdAsync(studentId);
        }

        // =====================================
        // CREATE STUDENT
        // =====================================

        public async Task<StudentResponse> CreateAsync(
            CreateStudentRequest request)
        {
            return await _repository.CreateAsync(request);
        }

        // =====================================
        // UPDATE STUDENT
        // =====================================

        public async Task<StudentResponse?> UpdateAsync(
            int studentId,
            UpdateStudentRequest request)
        {
            return await _repository.UpdateAsync(studentId, request);
        }

        // =====================================
        // DELETE STUDENT
        // =====================================

        public async Task<bool> DeleteAsync(int studentId)
        {
            return await _repository.DeleteAsync(studentId);
        }
        // =====================================
        // GET STUDENT PROFILE
        // =====================================

        public async Task<StudentProfileDto?> GetProfileAsync(
            int studentId)
        {
            return await _repository.GetProfileAsync(studentId);
        }

        // =====================================
        // UPDATE STUDENT PROFILE
        // =====================================

        public async Task<StudentProfileDto?> UpdateProfileAsync(
            int studentId,
            StudentProfileDto request)
        {
            return await _repository.UpdateProfileAsync(
                studentId,
                request);
        }

        // =====================================
        // CHANGE STUDENT SECTION
        // =====================================

        public async Task<bool> ChangeSectionAsync(
            int studentId,
            ChangeSectionRequest request)
        {
            return await _repository.ChangeSectionAsync(
                studentId,
                request);
        }

        // =====================================
        // CHANGE STUDENT GROUP
        // =====================================

        public async Task<bool> ChangeGroupAsync(
            int studentId,
            ChangeGroupRequest request)
        {
            return await _repository.ChangeGroupAsync(
                studentId,
                request);
        }

        // =====================================
        // TRANSFER STUDENT
        // =====================================

        public async Task<bool> TransferAsync(
            int studentId,
            TransferStudentRequest request)
        {
            return await _repository.TransferAsync(
                studentId,
                request);
        }
        // =====================================
        // SUSPEND STUDENT
        // =====================================

        public async Task<bool> SuspendAsync(
            int studentId,
            SuspendStudentRequest request)
        {
            return await _repository.SuspendAsync(
                studentId,
                request);
        }

        // =====================================
        // ACTIVATE STUDENT
        // =====================================

        public async Task<bool> ActivateAsync(int studentId)
        {
            return await _repository.ActivateAsync(studentId);
        }

        // =====================================
        // RESET PASSWORD
        // =====================================

        public async Task<bool> ResetPasswordAsync(int studentId)
        {
            return await _repository.ResetPasswordAsync(studentId);
        }

        // =====================================
        // STUDENT DASHBOARD
        // =====================================

        public async Task<StudentDashboardDto?> GetDashboardAsync(
            int studentId)
        {
            return await _repository.GetDashboardAsync(studentId);
        }
    }
}