using CollegeManagement.API.DTOs.StudentAdmission;

namespace CollegeManagement.API.Repositories.Interfaces
{
    public interface IStudentAdmissionRepository
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
            CreateStudentAdmissionRequest request,
            string? studentPhoto,
            string? birthCertificate,
            string? transferCertificate,
            string? studyCertificate,
            string? aadhaarDocument,
            string? communityCertificate,
            string? incomeCertificate,
            string? casteCertificate,
            string? tenthCertificate,
            string? marksMemo);


        // =========================================================
        // UPDATE
        // =========================================================

        Task<StudentAdmissionResponseDto?> UpdateAsync(
            int admissionId,
            UpdateStudentAdmissionRequest request,
            string? studentPhoto,
            string? birthCertificate,
            string? transferCertificate,
            string? studyCertificate,
            string? aadhaarDocument,
            string? communityCertificate,
            string? incomeCertificate,
            string? casteCertificate,
            string? tenthCertificate,
            string? marksMemo);


        // =========================================================
        // DELETE / SOFT DELETE
        // =========================================================

        Task<bool> DeleteAsync(
            int admissionId);
        //validatio//
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

        //GENERATE//
        Task<string> GenerateAdmissionNumberAsync();
    }
}