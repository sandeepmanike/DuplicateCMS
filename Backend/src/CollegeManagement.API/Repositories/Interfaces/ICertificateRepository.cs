using CollegeManagement.API.DTOs.Certificate;

namespace CollegeManagement.API.Repositories.Interfaces;

public interface ICertificateRepository
{
    Task<IReadOnlyList<CertificateResponseDto>> GetAllAsync(
        string? search,
        string? status,
        CancellationToken ct = default);

    Task<CertificateResponseDto?> GetByIdAsync(
        int id,
        CancellationToken ct = default);

    Task<CertificateResponseDto?> GenerateAsync(
        GenerateCertificateDto request,
        CancellationToken ct = default);

    Task<IReadOnlyList<CertificateResponseDto>> GetHistoryAsync(
        string? admissionNo,
        CancellationToken ct = default);

    Task<CertificateResponseDto?> VerifyAsync(
        string certificateNo,
        CancellationToken ct = default);

    Task<CertificateResponseDto?> ReissueAsync(
        ReissueCertificateDto request,
        CancellationToken ct = default);

    Task<CertificateResponseDto?> UpdateByAdmissionNoAsync(
        UpdateCertificateDto request,
        CancellationToken ct = default);

    Task<bool> CancelAsync(
        int id,
        CancellationToken ct = default);

    Task<bool> MoveStatusAsync(
        int id,
        string status,
        string? issuedBy = null,
        CancellationToken ct = default);

    Task<bool> DeleteAsync(
        int id,
        CancellationToken ct = default);
}