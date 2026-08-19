using CollegeManagement.API.DTOs.StudentAdmission;

namespace CollegeManagement.API.Services.Interfaces
{
    public interface IStudentAdmissionService
    {
        // =========================================================
        // GET ALL
        // =========================================================

        Task<IEnumerable<StudentAdmissionResponseDto>> GetAllAsync();


        // =========================================================
        // GET BY ID
        // =========================================================

        Task<StudentAdmissionResponseDto?> GetByIdAsync(
            int admissionId);


        // =========================================================
        // CREATE
        // =========================================================

        Task<StudentAdmissionResponseDto> CreateAsync(
            CreateStudentAdmissionRequest request);


        // =========================================================
        // UPDATE
        // =========================================================

        Task<StudentAdmissionResponseDto?> UpdateAsync(
            int admissionId,
            UpdateStudentAdmissionRequest request);


        // =========================================================
        // DELETE / SOFT DELETE
        // =========================================================

        Task<bool> DeleteAsync(
            int admissionId);
        //validation//
        Task<dynamic?> SubmitAsync(int admissionId);

        // =========================================================
        // VERIFY
        // =========================================================

        Task<bool> VerifyAsync(
            int admissionId);


        // =========================================================
        // APPROVE
        // Creates Student + generates RollNo
        // =========================================================

        Task<AdmissionApprovalResponseDto?> ApproveAsync(
            int admissionId);


        // =========================================================
        // REJECT
        // =========================================================

        Task<bool> RejectAsync(
            int admissionId);

        //GENEATE//
        Task<string> GenerateAdmissionNumberAsync();
    }
}