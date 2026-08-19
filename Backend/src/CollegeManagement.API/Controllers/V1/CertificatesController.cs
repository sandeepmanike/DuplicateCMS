using CollegeManagement.API.DTOs.Certificate;
using CollegeManagement.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace CollegeManagement.API.Controllers.V1;

[ApiController]
[Authorize]
[Route("api/certificates")]
public class CertificatesController : ControllerBase
{
    private readonly ICertificateService _service;

    public CertificatesController(ICertificateService service)
    {
        _service = service;
    }

    // =========================================================
    // GET ALL
    // GET /api/certificates
    // =========================================================

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? search = null,
        [FromQuery] string? status = null,
        CancellationToken ct = default)
    {
        var result = await _service.GetAllAsync(
            search,
            status,
            ct);

        return Ok(result);
    }

    // =========================================================
    // GET BY ID
    // GET /api/certificates/{id}
    // =========================================================

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(
        int id,
        CancellationToken ct = default)
    {
        if (id <= 0)
        {
            return BadRequest(new
            {
                message = "Invalid certificate ID"
            });
        }

        var result = await _service.GetByIdAsync(
            id,
            ct);

        if (result == null)
        {
            return NotFound(new
            {
                message = "Certificate not found"
            });
        }

        return Ok(result);
    }

    // =========================================================
    // BONAFIDE
    // POST /api/certificates/bonafide
    // =========================================================

    [HttpPost("bonafide")]
    public async Task<IActionResult> Bonafide(
        [FromBody] GenerateCertificateRequestDto request,
        CancellationToken ct = default)
    {
        if (request == null)
        {
            return BadRequest(new
            {
                message = "Request body is required"
            });
        }

        if (string.IsNullOrWhiteSpace(request.AdmissionNo))
        {
            return BadRequest(new
            {
                message = "AdmissionNo is required"
            });
        }

        if (string.IsNullOrWhiteSpace(request.Purpose))
        {
            return BadRequest(new
            {
                message = "Purpose is required"
            });
        }

        var result = await _service.GenerateAsync(
            "Bonafide",
            request,
            ct);

        if (result == null)
        {
            return BadRequest(new
            {
                message =
                    $"Unable to generate Bonafide certificate for AdmissionNo '{request.AdmissionNo}'."
            });
        }

        return Ok(result);
    }

    // =========================================================
    // STUDY
    // POST /api/certificates/study
    // =========================================================

    [HttpPost("study")]
    public async Task<IActionResult> Study(
        [FromBody] GenerateCertificateRequestDto request,
        CancellationToken ct = default)
    {
        if (request == null)
        {
            return BadRequest(new
            {
                message = "Request body is required"
            });
        }

        if (string.IsNullOrWhiteSpace(request.AdmissionNo))
        {
            return BadRequest(new
            {
                message = "AdmissionNo is required"
            });
        }

        if (string.IsNullOrWhiteSpace(request.Purpose))
        {
            return BadRequest(new
            {
                message = "Purpose is required"
            });
        }

        var result = await _service.GenerateAsync(
            "Study",
            request,
            ct);

        if (result == null)
        {
            return BadRequest(new
            {
                message =
                    $"Unable to generate Study certificate for AdmissionNo '{request.AdmissionNo}'."
            });
        }

        return Ok(result);
    }

    // =========================================================
    // CONDUCT
    // POST /api/certificates/conduct
    // =========================================================

    [HttpPost("conduct")]
    public async Task<IActionResult> Conduct(
        [FromBody] GenerateCertificateRequestDto request,
        CancellationToken ct = default)
    {
        if (request == null)
        {
            return BadRequest(new
            {
                message = "Request body is required"
            });
        }

        if (string.IsNullOrWhiteSpace(request.AdmissionNo))
        {
            return BadRequest(new
            {
                message = "AdmissionNo is required"
            });
        }

        if (string.IsNullOrWhiteSpace(request.Purpose))
        {
            return BadRequest(new
            {
                message = "Purpose is required"
            });
        }

        var result = await _service.GenerateAsync(
            "Conduct",
            request,
            ct);

        if (result == null)
        {
            return BadRequest(new
            {
                message =
                    $"Unable to generate Conduct certificate for AdmissionNo '{request.AdmissionNo}'."
            });
        }

        return Ok(result);
    }

    // =========================================================
    // TRANSFER CERTIFICATE
    // POST /api/certificates/tc
    // =========================================================

    [HttpPost("tc")]
    public async Task<IActionResult> TransferCertificate(
        [FromBody] GenerateCertificateRequestDto request,
        CancellationToken ct = default)
    {
        if (request == null)
        {
            return BadRequest(new
            {
                message = "Request body is required"
            });
        }

        if (string.IsNullOrWhiteSpace(request.AdmissionNo))
        {
            return BadRequest(new
            {
                message = "AdmissionNo is required"
            });
        }

        if (string.IsNullOrWhiteSpace(request.Purpose))
        {
            return BadRequest(new
            {
                message = "Purpose is required"
            });
        }

        var result = await _service.GenerateAsync(
            "Transfer Certificate",
            request,
            ct);

        if (result == null)
        {
            return BadRequest(new
            {
                message =
                    $"Unable to generate Transfer Certificate for AdmissionNo '{request.AdmissionNo}'."
            });
        }

        return Ok(result);
    }

    // =========================================================
    // OTHER CERTIFICATE
    // POST /api/certificates/other
    // =========================================================
    [HttpPost("other")]
    public async Task<IActionResult> OtherCertificate(
        [FromBody] GenerateCertificateRequestDto request,
        CancellationToken ct = default)
    {
        if (request == null)
        {
            return BadRequest(new
            {
                message = "Request body is required"
            });
        }

        if (string.IsNullOrWhiteSpace(request.AdmissionNo))
        {
            return BadRequest(new
            {
                message = "AdmissionNo is required"
            });
        }

        if (string.IsNullOrWhiteSpace(request.CertificateType))
        {
            return BadRequest(new
            {
                message = "CertificateType is required"
            });
        }

        if (string.IsNullOrWhiteSpace(request.Purpose))
        {
            return BadRequest(new
            {
                message = "Purpose is required"
            });
        }

        var result = await _service.GenerateAsync(
            request.CertificateType.Trim(),
            request,
            ct);

        if (result == null)
        {
            return BadRequest(new
            {
                message =
                    $"Unable to generate {request.CertificateType} certificate for AdmissionNo '{request.AdmissionNo}'."
            });
        }

        return Ok(result);
    }

    // =========================================================
    // HISTORY
    // GET /api/certificates/history
    // =========================================================

    [HttpGet("history")]
    public async Task<IActionResult> History(
        [FromQuery] string? admissionNo = null,
        CancellationToken ct = default)
    {
        var result =
            await _service.GetHistoryAsync(
                admissionNo,
                ct);

        return Ok(result);
    }

    // =========================================================
    // VERIFY
    // GET /api/certificates/verify/{certificateNo}
    // =========================================================

    [HttpGet("verify/{certificateNo}")]
    [AllowAnonymous]
    public async Task<IActionResult> Verify(
        string certificateNo,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(certificateNo))
        {
            return BadRequest(new
            {
                message = "Certificate number is required"
            });
        }

        var result =
            await _service.VerifyAsync(
                certificateNo,
                ct);

        if (result == null)
        {
            return NotFound(new
            {
                message =
                    "Certificate not found or inactive"
            });
        }

        return Ok(result);
    }

    // =========================================================
    // REISSUE
    // POST /api/certificates/reissue
    // =========================================================

    [HttpPost("reissue")]
    public async Task<IActionResult> Reissue(
        [FromBody] ReissueCertificateDto request,
        CancellationToken ct = default)
    {
        if (request == null)
        {
            return BadRequest(new
            {
                message = "Request body is required."
            });
        }

        if (string.IsNullOrWhiteSpace(request.AdmissionNo))
        {
            return BadRequest(new
            {
                message = "AdmissionNo is required."
            });
        }

        if (string.IsNullOrWhiteSpace(request.CertificateType))
        {
            return BadRequest(new
            {
                message = "CertificateType is required."
            });
        }

        var result =
            await _service.ReissueAsync(
                request,
                ct);

        if (result == null)
        {
            return NotFound(new
            {
                message =
                    $"Unable to reissue certificate for AdmissionNo '{request.AdmissionNo}'."
            });
        }

        return Ok(result);
    }

    // =========================================================
    // CANCEL
    // PATCH /api/certificates/{id}/cancel
    // =========================================================

    [HttpPatch("{id:int}/cancel")]
    public async Task<IActionResult> Cancel(
        int id,
        CancellationToken ct = default)
    {
        if (id <= 0)
        {
            return BadRequest(new
            {
                message = "Invalid certificate ID"
            });
        }

        var success =
            await _service.CancelAsync(
                id,
                ct);

        if (!success)
        {
            return NotFound(new
            {
                message = "Certificate not found"
            });
        }

        return Ok(new
        {
            success = true,
            message = "Certificate cancelled successfully"
        });
    }

    // =========================================================
    // REVIEW
    // PATCH /api/certificates/{id}/review
    // =========================================================

    [HttpPatch("{id:int}/review")]
    public async Task<IActionResult> Review(
        int id,
        CancellationToken ct = default)
    {
        if (id <= 0)
        {
            return BadRequest(new
            {
                message = "Invalid certificate ID"
            });
        }

        var success =
            await _service.MoveStatusAsync(
                id,
                "Reviewed",
                User.Identity?.Name,
                ct);

        if (!success)
        {
            return NotFound(new
            {
                message = "Certificate not found"
            });
        }

        return Ok(new
        {
            success = true,
            message = "Certificate reviewed successfully"
        });
    }

    // =========================================================
    // APPROVE
    // PATCH /api/certificates/{id}/approve
    // =========================================================

    [HttpPatch("{id:int}/approve")]
    public async Task<IActionResult> Approve(
        int id,
        CancellationToken ct = default)
    {
        if (id <= 0)
        {
            return BadRequest(new
            {
                message = "Invalid certificate ID"
            });
        }

        var success =
            await _service.MoveStatusAsync(
                id,
                "Approved",
                User.Identity?.Name,
                ct);

        if (!success)
        {
            return NotFound(new
            {
                message = "Certificate not found"
            });
        }

        return Ok(new
        {
            success = true,
            message = "Certificate approved successfully"
        });
    }

    // =========================================================
    // ISSUE
    // PATCH /api/certificates/{id}/issue
    // =========================================================

    [HttpPatch("{id:int}/issue")]
    public async Task<IActionResult> Issue(
        int id,
        [FromQuery] string? issuedBy = null,
        CancellationToken ct = default)
    {
        if (id <= 0)
        {
            return BadRequest(new
            {
                message = "Invalid certificate ID"
            });
        }

        var success =
            await _service.MoveStatusAsync(
                id,
                "Issued",
                issuedBy ?? User.Identity?.Name,
                ct);

        if (!success)
        {
            return NotFound(new
            {
                message = "Certificate not found"
            });
        }

        return Ok(new
        {
            success = true,
            message = "Certificate issued successfully"
        });
    }

    // =========================================================
    // DELETE
    // DELETE /api/certificates/{id}
    // =========================================================

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(
        int id,
        CancellationToken ct = default)
    {
        if (id <= 0)
        {
            return BadRequest(new
            {
                message = "Invalid certificate ID"
            });
        }

        var success =
            await _service.DeleteAsync(
                id,
                ct);

        if (!success)
        {
            return NotFound(new
            {
                message = "Certificate not found"
            });
        }

        return Ok(new
        {
            success = true,
            message = "Certificate deleted successfully"
        });
    }

    // =========================================================
    // DOWNLOAD CERTIFICATE
    // GET /api/certificates/download/{id}
    // =========================================================

    [HttpGet("download/{id:int}")]
    public async Task<IActionResult> Download(
        int id,
        CancellationToken ct = default)
    {
        if (id <= 0)
        {
            return BadRequest(new
            {
                message = "Invalid certificate ID"
            });
        }

        var certificate =
            await _service.GetByIdAsync(
                id,
                ct);

        if (certificate == null)
        {
            return NotFound(new
            {
                message = "Certificate not found"
            });
        }

        var signaturePath = Path.Combine(
            Directory.GetCurrentDirectory(),
            "wwwroot",
            "images",
            "signature.png");

        if (!System.IO.File.Exists(signaturePath))
        {
            return NotFound(new
            {
                message =
                    $"Signature image not found at: {signaturePath}"
            });
        }

        var bytes = BuildCertificatePdf(
            certificate,
            signaturePath);

        return File(
            bytes,
            "application/pdf",
            $"{certificate.CertificateNumber}.pdf");
    }

    // =========================================================
    // BUILD CERTIFICATE PDF
    // =========================================================

    private static byte[] BuildCertificatePdf(
        CertificateResponseDto certificate,
        string signaturePath)
    {
        QuestPDF.Settings.License =
            LicenseType.Community;

        var document =
            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);

                    page.Margin(50);

                    page.Content()
                        .Column(column =>
                        {
                            column.Spacing(10);

                            column.Item()
                                .AlignCenter()
                                .Text(
                                    "COLLEGE MANAGEMENT SYSTEM")
                                .Bold()
                                .FontSize(20);

                            column.Item()
                                .AlignCenter()
                                .Text(
                                    certificate.CertificateType ?? "")
                                .Bold()
                                .FontSize(17);

                            column.Item()
                                .PaddingTop(25)
                                .Text(
                                    $"Certificate No: {certificate.CertificateNumber}");

                            column.Item()
                                .Text(
                                    $"Student: {certificate.StudentName}");

                            column.Item()
                                .Text(
                                    $"Admission No: {certificate.AdmissionNo}");

                            column.Item()
                                .Text(
                                    $"Academic Level: {certificate.AcademicLevel}");

                            column.Item()
                                .Text(
                                    $"Purpose: {certificate.Purpose}");

                            column.Item()
                                .Text(
                                    $"Status: {certificate.Status}");

                            column.Item()
                                .Text(
                                    $"Issue Date: {certificate.IssueDate:yyyy-MM-dd}");

                            column.Item()
                                .Text(
                                    $"Remarks: {certificate.Remarks}");
                        });

                    page.Footer()
                        .AlignRight()
                        .Column(signatureColumn =>
                        {
                            signatureColumn
                                .Item()
                                .AlignCenter()
                                .Width(160)
                                .Height(70)
                                .Image(signaturePath);

                            signatureColumn
                                .Item()
                                .AlignCenter()
                                .Text(
                                    "Authorized Signature")
                                .FontSize(10);
                        });
                });
            });

        return document.GeneratePdf();
    }
}