using CollegeManagement.API.DTOs.Students;
using CollegeManagement.API.Exceptions;
using CollegeManagement.API.Repositories;
using CollegeManagement.API.Services;

namespace CollegeManagement.API.Services.Implementations
{
    public class StudentService : IStudentService
    {
        private readonly IStudentRepository _repository;

        public StudentService(IStudentRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<StudentListItemDto>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<StudentResponse?> GetByIdAsync(int studentId)
        {
            return await _repository.GetByIdAsync(studentId);
        }

        public async Task<StudentResponse?> UpdateAsync(int studentId, UpdateStudentRequest request)
        {
            if (!string.IsNullOrWhiteSpace(request.Email) && await _repository.EmailExistsAsync(request.Email, studentId))
            {
                throw new ConflictException("A student with this email address already exists.");
            }

            if (!string.IsNullOrWhiteSpace(request.MobileNumber) && await _repository.MobileExistsAsync(request.MobileNumber, studentId))
            {
                throw new ConflictException("A student with this mobile number already exists.");
            }

            return await _repository.UpdateAsync(studentId, request);
        }

        public async Task<bool> DeleteAsync(int studentId)
        {
            return await _repository.DeleteAsync(studentId);
        }

        public async Task<StudentProfileDto?> GetProfileAsync(int studentId)
        {
            return await _repository.GetProfileAsync(studentId);
        }

        public async Task<StudentProfileDto?> UpdateProfileAsync(int studentId, UpdateStudentProfileRequest request)
        {
            if (!string.IsNullOrWhiteSpace(request.Email) && await _repository.EmailExistsAsync(request.Email, studentId))
            {
                throw new ConflictException("A student with this email address already exists.");
            }

            if (!string.IsNullOrWhiteSpace(request.MobileNumber) && await _repository.MobileExistsAsync(request.MobileNumber, studentId))
            {
                throw new ConflictException("A student with this mobile number already exists.");
            }

            return await _repository.UpdateProfileAsync(studentId, request);
        }

        public async Task<bool> ChangeSectionAsync(int studentId, ChangeSectionRequest request)
        {
            return await _repository.ChangeSectionAsync(studentId, request);
        }

        public async Task<bool> ChangeGroupAsync(int studentId, ChangeGroupRequest request)
        {
            return await _repository.ChangeGroupAsync(studentId, request);
        }

        public async Task<bool> TransferAsync(int studentId, TransferStudentRequest request)
        {
            return await _repository.TransferAsync(studentId, request);
        }

        public async Task<bool> SuspendAsync(int studentId, SuspendStudentRequest request)
        {
            return await _repository.SuspendAsync(studentId, request);
        }

        public async Task<bool> ActivateAsync(int studentId)
        {
            return await _repository.ActivateAsync(studentId);
        }

        public async Task<bool> ResetPasswordAsync(int studentId)
        {
            return await _repository.ResetPasswordAsync(studentId);
        }

        public async Task<StudentDashboardDto?> GetDashboardAsync(int studentId)
        {
            return await _repository.GetDashboardAsync(studentId);
        }

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

        public async Task<List<StudentListItemDto>> GetByGroupAsync(int groupId)
        {
            return await _repository.GetByGroupAsync(groupId);
        }

        public async Task<List<StudentListItemDto>> GetBySectionAsync(int sectionId)
        {
            return await _repository.GetBySectionAsync(sectionId);
        }

        public async Task<List<StudentListItemDto>> GetActiveAsync()
        {
            return await _repository.GetActiveAsync();
        }

        public async Task<bool> EmailExistsAsync(string email, int? excludeStudentId = null)
        {
            return await _repository.EmailExistsAsync(email, excludeStudentId);
        }

        public async Task<bool> MobileExistsAsync(string mobile, int? excludeStudentId = null)
        {
            return await _repository.MobileExistsAsync(mobile, excludeStudentId);
        }
    }
}
