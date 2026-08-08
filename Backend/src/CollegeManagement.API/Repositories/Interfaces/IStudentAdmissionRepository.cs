using CollegeManagement.API.DTOs.StudentAdmission;

namespace CollegeManagement.API.Repositories.Interfaces
{
    public interface IStudentAdmissionRepository
    {
        Task<IEnumerable<StudentAdmissionResponseDto>> GetAllAsync();

        Task<StudentAdmissionResponseDto?> GetByIdAsync(int admissionId);

        Task<StudentAdmissionResponseDto> CreateAsync(CreateStudentAdmissionRequest request);

        Task<StudentAdmissionResponseDto?> UpdateAsync(
            int admissionId,
            UpdateStudentAdmissionRequest request);

        Task<bool> DeleteAsync(int admissionId);

        Task<bool> VerifyAsync(int admissionId);

        Task<bool> ApproveAsync(int admissionId);

        Task<bool> RejectAsync(int admissionId);

        Task<string> GenerateAdmissionNumberAsync();
    }
}