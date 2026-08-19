using CollegeManagement.API.DTOs.Certificate;
using CollegeManagement.API.Repositories.Interfaces;
using CollegeManagement.API.Services.Interfaces;

namespace CollegeManagement.API.Services;

public class CertificateService : ICertificateService
{
    private readonly ICertificateRepository _repository;

    public CertificateService(
        ICertificateRepository repository)
    {
        _repository = repository;
    }

    // =========================================================
    // GET ALL
    // =========================================================

    public async Task<IReadOnlyList<CertificateResponseDto>> GetAllAsync(
        string? search,
        string? status,
        CancellationToken ct = default)
    {
        return await _repository.GetAllAsync(
            search,
            status,
            ct);
    }

    // =========================================================
    // GET BY ID
    // =========================================================

    public async Task<CertificateResponseDto?> GetByIdAsync(
        int id,
        CancellationToken ct = default)
    {
        return await _repository.GetByIdAsync(
            id,
            ct);
    }

    // =========================================================
    // GENERATE FROM DTO
    // =========================================================

    public async Task<CertificateResponseDto?> GenerateAsync(
        GenerateCertificateDto request,
        CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (string.IsNullOrWhiteSpace(request.AdmissionNo))
        {
            throw new ArgumentException(
                "AdmissionNo is required.");
        }

        if (string.IsNullOrWhiteSpace(request.CertificateType))
        {
            throw new ArgumentException(
                "CertificateType is required.");
        }

        return await _repository.GenerateAsync(
            request,
            ct);
    }

    // =========================================================
    // GENERATE FROM REQUEST
    // =========================================================

    public async Task<CertificateResponseDto?> GenerateAsync(
        string certificateType,
        GenerateCertificateRequestDto request,
        CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (string.IsNullOrWhiteSpace(certificateType))
        {
            throw new ArgumentException(
                "Certificate type is required.");
        }

        if (string.IsNullOrWhiteSpace(request.AdmissionNo))
        {
            throw new ArgumentException(
                "AdmissionNo is required.");
        }

        var issueDate =
            request.IssueDate == default
                ? DateTime.UtcNow
                : request.IssueDate;

        var dto = new GenerateCertificateDto
        {
            AdmissionNo =
                request.AdmissionNo.Trim(),

            CertificateType =
                certificateType,

            Purpose =
                request.Purpose?.Trim() ?? string.Empty,

            IssueDate =
                issueDate,

            Remarks =
                request.Remarks?.Trim()
        };

        return await _repository.GenerateAsync(
            dto,
            ct);
    }

    // =========================================================
    // UPDATE
    // =========================================================

    public async Task<CertificateResponseDto?> UpdateByAdmissionNoAsync(
        UpdateCertificateDto request,
        CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (string.IsNullOrWhiteSpace(request.AdmissionNo))
        {
            throw new ArgumentException(
                "AdmissionNo is required.");
        }

        return await _repository.UpdateByAdmissionNoAsync(
            request,
            ct);
    }

    // =========================================================
    // HISTORY
    // =========================================================

    public async Task<IReadOnlyList<CertificateResponseDto>> GetHistoryAsync(
        string? admissionNo,
        CancellationToken ct = default)
    {
        return await _repository.GetHistoryAsync(
            admissionNo,
            ct);
    }

    // =========================================================
    // VERIFY
    // =========================================================

    public async Task<CertificateResponseDto?> VerifyAsync(
        string certificateNo,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(certificateNo))
        {
            throw new ArgumentException(
                "Certificate number is required.");
        }

        return await _repository.VerifyAsync(
            certificateNo.Trim(),
            ct);
    }

    // =========================================================
    // REISSUE
    // =========================================================

    public async Task<CertificateResponseDto?> ReissueAsync(
        ReissueCertificateDto request,
        CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (string.IsNullOrWhiteSpace(request.AdmissionNo))
        {
            throw new ArgumentException(
                "AdmissionNo is required.");
        }

        return await _repository.ReissueAsync(
            request,
            ct);
    }

    // =========================================================
    // STATUS
    // =========================================================

    public async Task<bool> MoveStatusAsync(
        int id,
        string status,
        string? issuedBy = null,
        CancellationToken ct = default)
    {
        if (id <= 0)
            return false;

        if (string.IsNullOrWhiteSpace(status))
        {
            throw new ArgumentException(
                "Status is required.");
        }

        return await _repository.MoveStatusAsync(
            id,
            status,
            issuedBy,
            ct);
    }

    // =========================================================
    // CANCEL
    // =========================================================

    public async Task<bool> CancelAsync(
        int id,
        CancellationToken ct = default)
    {
        if (id <= 0)
            return false;

        return await _repository.CancelAsync(
            id,
            ct);
    }

    // =========================================================
    // DELETE
    // =========================================================

    public async Task<bool> DeleteAsync(
        int id,
        CancellationToken ct = default)
    {
        if (id <= 0)
            return false;

        return await _repository.DeleteAsync(
            id,
            ct);
    }
}