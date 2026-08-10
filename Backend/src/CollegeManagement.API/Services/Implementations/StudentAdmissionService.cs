using CollegeManagement.API.DTOs.StudentAdmission;
using CollegeManagement.API.Repositories.Interfaces;
using CollegeManagement.API.Services.Interfaces;

namespace CollegeManagement.API.Services.Implementations
{
    public class StudentAdmissionService : IStudentAdmissionService
    {
        private readonly IStudentAdmissionRepository _repository;

        public StudentAdmissionService(
            IStudentAdmissionRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<StudentAdmissionResponseDto>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<StudentAdmissionResponseDto?> GetByIdAsync(int admissionId)
        {
            return await _repository.GetByIdAsync(admissionId);
        }

        public async Task<StudentAdmissionResponseDto> CreateAsync(
            CreateStudentAdmissionRequest request)
        {
            return await _repository.CreateAsync(request);
        }

        public async Task<StudentAdmissionResponseDto?> UpdateAsync(
            int admissionId,
            UpdateStudentAdmissionRequest request)
        {
            return await _repository.UpdateAsync(
                admissionId,
                request);
        }

        public async Task<bool> DeleteAsync(int admissionId)
        {
            return await _repository.DeleteAsync(admissionId);
        }

        public async Task<bool> VerifyAsync(int admissionId)
        {
            return await _repository.VerifyAsync(admissionId);
        }

        public async Task<bool> ApproveAsync(int admissionId)
        {
            return await _repository.ApproveAsync(admissionId);
        }

        public async Task<bool> RejectAsync(int admissionId)
        {
            return await _repository.RejectAsync(admissionId);
        }

        public async Task<string> GenerateAdmissionNumberAsync()
        {
            return await _repository.GenerateAdmissionNumberAsync();
        }
    }
}