using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Asp.Versioning;
using CollegeManagement.API.DTOs.Certificate;
using CollegeManagement.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace CollegeManagement.API.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/certificates")]
[Authorize]
[Produces("application/json")]
public class CertificatesController : ControllerBase
{
    private readonly ICertificateService _service;

    public CertificatesController(ICertificateService service)
    {
        _service = service;
    }

    // =========================================================
    // 1. GET ALL CERTIFICATES
    // GET /api/v1/certificates
    // =========================================================
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<CertificateResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? search = null,
        [FromQuery] string? status = null,
        [FromQuery] string? certificateType = null,
        CancellationToken ct = default)
    {
        var result = await _service.GetAllAsync(search, status, certificateType, ct);
        return Ok(result);
    }

    // =========================================================
    // 2. GET WORKFLOW STATS (5 Stage Count Badges)
    // GET /api/v1/certificates/workflow-stats
    // =========================================================
    [HttpGet("workflow-stats")]
    [ProducesResponseType(typeof(CertificateWorkflowStatsDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetWorkflowStats(CancellationToken ct = default)
    {
        var stats = await _service.GetWorkflowStatsAsync(ct);
        return Ok(stats);
    }

    // =========================================================
    // 3. GET STUDENTS DROPDOWN (For Create Certificate Auto-fill)
    // GET /api/v1/certificates/students-dropdown
    // =========================================================
    [HttpGet("students-dropdown")]
    [ProducesResponseType(typeof(IReadOnlyList<StudentCertificateDropdownDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStudentsDropdown(CancellationToken ct = default)
    {
        var students = await _service.GetStudentsDropdownAsync(ct);
        return Ok(students);
    }

    // =========================================================
    // 4. GET BY ID
    // GET /api/v1/certificates/{id}
    // =========================================================
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(CertificateResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id, CancellationToken ct = default)
    {
        if (id <= 0) return BadRequest(new { message = "Invalid certificate ID" });

        var result = await _service.GetByIdAsync(id, ct);
        if (result == null) return NotFound(new { message = "Certificate not found" });

        return Ok(result);
    }

    // =========================================================
    // 5. UNIFIED GENERATE CERTIFICATE
    // POST /api/v1/certificates/generate
    // =========================================================
    [HttpPost("generate")]
    [ProducesResponseType(typeof(CertificateResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Generate(
        [FromBody] GenerateCertificateRequestDto request,
        CancellationToken ct = default)
    {
        if (request == null) return BadRequest(new { message = "Request body is required" });
        if (string.IsNullOrWhiteSpace(request.AdmissionNo)) return BadRequest(new { message = "Admission number is required" });
        if (string.IsNullOrWhiteSpace(request.CertificateType)) return BadRequest(new { message = "Certificate type is required" });
        if (string.IsNullOrWhiteSpace(request.Purpose)) return BadRequest(new { message = "Purpose is required" });

        var result = await _service.GenerateAsync(request, ct);
        if (result == null)
            return BadRequest(new { message = $"Unable to generate {request.CertificateType} certificate for AdmissionNo '{request.AdmissionNo}'." });

        return Ok(result);
    }

    // =========================================================
    // 6. SPECIFIC GENERATE ENDPOINTS (Backwards Compatibility)
    // =========================================================
    [HttpPost("bonafide")]
    public async Task<IActionResult> Bonafide([FromBody] GenerateCertificateRequestDto request, CancellationToken ct = default)
        => await GenerateType("Bonafide Certificate", request, ct);

    [HttpPost("study")]
    public async Task<IActionResult> Study([FromBody] GenerateCertificateRequestDto request, CancellationToken ct = default)
        => await GenerateType("Study Certificate", request, ct);

    [HttpPost("conduct")]
    public async Task<IActionResult> Conduct([FromBody] GenerateCertificateRequestDto request, CancellationToken ct = default)
        => await GenerateType("Conduct Certificate", request, ct);

    [HttpPost("tc")]
    public async Task<IActionResult> TransferCertificate([FromBody] GenerateCertificateRequestDto request, CancellationToken ct = default)
        => await GenerateType("Transfer Certificate", request, ct);

    [HttpPost("other")]
    public async Task<IActionResult> OtherCertificate([FromBody] GenerateCertificateRequestDto request, CancellationToken ct = default)
    {
        var type = !string.IsNullOrWhiteSpace(request.CertificateType) ? request.CertificateType : "General Certificate";
        return await GenerateType(type, request, ct);
    }

    private async Task<IActionResult> GenerateType(string type, GenerateCertificateRequestDto request, CancellationToken ct)
    {
        if (request == null) return BadRequest(new { message = "Request body is required" });
        request.CertificateType = type;
        var result = await _service.GenerateAsync(request, ct);
        if (result == null) return BadRequest(new { message = $"Unable to generate {type} certificate for '{request.AdmissionNo}'." });
        return Ok(result);
    }

    // =========================================================
    // 7. REVIEW CERTIFICATE (Stage 1 -> 2)
    // PATCH /api/v1/certificates/{id}/review
    // =========================================================
    [HttpPatch("{id:int}/review")]
    public async Task<IActionResult> Review(int id, CancellationToken ct = default)
    {
        if (id <= 0) return BadRequest(new { message = "Invalid certificate ID" });

        var userName = User.Identity?.Name ?? "Admin";
        var success = await _service.MoveStatusAsync(id, "Reviewed", userName, ct);
        if (!success) return NotFound(new { message = "Certificate not found" });

        return Ok(new { success = true, message = "Certificate reviewed successfully" });
    }

    // =========================================================
    // 8. APPROVE CERTIFICATE (Stage 2 -> 3)
    // PATCH /api/v1/certificates/{id}/approve
    // =========================================================
    [HttpPatch("{id:int}/approve")]
    public async Task<IActionResult> Approve(int id, CancellationToken ct = default)
    {
        if (id <= 0) return BadRequest(new { message = "Invalid certificate ID" });

        var userName = User.Identity?.Name ?? "Principal";
        var success = await _service.MoveStatusAsync(id, "Approved", userName, ct);
        if (!success) return NotFound(new { message = "Certificate not found" });

        return Ok(new { success = true, message = "Certificate approved successfully" });
    }

    // =========================================================
    // 9. ISSUE CERTIFICATE (Stage 3 -> 4)
    // PATCH /api/v1/certificates/{id}/issue
    // =========================================================
    [HttpPatch("{id:int}/issue")]
    public async Task<IActionResult> Issue(int id, [FromQuery] string? issuedBy = null, CancellationToken ct = default)
    {
        if (id <= 0) return BadRequest(new { message = "Invalid certificate ID" });

        var issuer = issuedBy ?? User.Identity?.Name ?? "Principal";
        var success = await _service.MoveStatusAsync(id, "Issued", issuer, ct);
        if (!success) return NotFound(new { message = "Certificate not found" });

        return Ok(new { success = true, message = "Certificate issued successfully" });
    }

    // =========================================================
    // 10. BULK APPROVE (Approve All Reviewed)
    // PATCH /api/v1/certificates/bulk-approve
    // =========================================================
    [HttpPatch("bulk-approve")]
    public async Task<IActionResult> BulkApprove(CancellationToken ct = default)
    {
        var userName = User.Identity?.Name ?? "Principal";
        var affected = await _service.BulkApproveAsync(userName, ct);
        return Ok(new { success = true, count = affected, message = $"{affected} certificate(s) approved successfully." });
    }

    // =========================================================
    // 11. BULK ISSUE (Issue All Approved)
    // PATCH /api/v1/certificates/bulk-issue
    // =========================================================
    [HttpPatch("bulk-issue")]
    public async Task<IActionResult> BulkIssue([FromQuery] string? issuedBy = null, CancellationToken ct = default)
    {
        var issuer = issuedBy ?? User.Identity?.Name ?? "Principal";
        var affected = await _service.BulkIssueAsync(issuer, ct);
        return Ok(new { success = true, count = affected, message = $"{affected} certificate(s) issued successfully." });
    }

    // =========================================================
    // 12. CANCEL CERTIFICATE (Stage -> Cancelled)
    // PATCH /api/v1/certificates/{id}/cancel
    // =========================================================
    [HttpPatch("{id:int}/cancel")]
    public async Task<IActionResult> Cancel(int id, CancellationToken ct = default)
    {
        if (id <= 0) return BadRequest(new { message = "Invalid certificate ID" });

        var success = await _service.CancelAsync(id, ct);
        if (!success) return NotFound(new { message = "Certificate not found" });

        return Ok(new { success = true, message = "Certificate cancelled successfully" });
    }

    // =========================================================
    // 13. DELETE CERTIFICATE (Soft Delete)
    // DELETE /api/v1/certificates/{id}
    // =========================================================
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct = default)
    {
        if (id <= 0) return BadRequest(new { message = "Invalid certificate ID" });

        var success = await _service.DeleteAsync(id, ct);
        if (!success) return NotFound(new { message = "Certificate not found" });

        return Ok(new { success = true, message = "Certificate deleted successfully" });
    }

    // =========================================================
    // 14. REISSUE CERTIFICATE
    // POST /api/v1/certificates/reissue
    // =========================================================
    [HttpPost("reissue")]
    public async Task<IActionResult> Reissue([FromBody] ReissueCertificateDto request, CancellationToken ct = default)
    {
        if (request == null) return BadRequest(new { message = "Request body is required." });
        if (string.IsNullOrWhiteSpace(request.AdmissionNo)) return BadRequest(new { message = "AdmissionNo is required." });

        var result = await _service.ReissueAsync(request, ct);
        if (result == null) return NotFound(new { message = $"Unable to reissue certificate for AdmissionNo '{request.AdmissionNo}'." });

        return Ok(result);
    }

    // =========================================================
    // 15. VERIFY CERTIFICATE (Public Link)
    // GET /api/v1/certificates/verify/{certificateNo}
    // =========================================================
    [HttpGet("verify/{certificateNo}")]
    [AllowAnonymous]
    public async Task<IActionResult> Verify(string certificateNo, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(certificateNo)) return BadRequest(new { message = "Certificate number is required" });

        var result = await _service.VerifyAsync(certificateNo, ct);
        if (result == null) return NotFound(new { message = "Certificate not found or cancelled" });

        return Ok(result);
    }

    // =========================================================
    // 16. DOWNLOAD CERTIFICATE PDF
    // GET /api/v1/certificates/download/{id}
    // =========================================================
    [HttpGet("download/{id:int}")]
    public async Task<IActionResult> Download(int id, CancellationToken ct = default)
    {
        if (id <= 0) return BadRequest(new { message = "Invalid certificate ID" });

        var certificate = await _service.GetByIdAsync(id, ct);
        if (certificate == null) return NotFound(new { message = "Certificate not found" });

        var signaturePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "signature.png");
        var hasSignature = System.IO.File.Exists(signaturePath);

        var bytes = BuildCertificatePdf(certificate, hasSignature ? signaturePath : null);

        return File(bytes, "application/pdf", $"{certificate.CertificateNumber}.pdf");
    }

    // =========================================================
    // QUESTPDF STYLED CERTIFICATE BUILDER
    // =========================================================
    private static byte[] BuildCertificatePdf(CertificateResponseDto certificate, string? signaturePath)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(40);

                page.Content().Border(3).BorderColor("#2e7d32").Padding(24).Column(column =>
                {
                    column.Spacing(8);

                    // Top Header
                    column.Item().AlignCenter().Text("COLLEGE MANAGEMENT SYSTEM").Bold().FontSize(22).FontColor("#1b5e20");
                    column.Item().AlignCenter().Text("Recognized by State Board of Intermediate Education").FontSize(11).FontColor("#555555");
                    column.Item().PaddingTop(6).LineHorizontal(1.5f).LineColor("#2e7d32");

                    // Certificate Title Banner
                    column.Item().PaddingTop(16).AlignCenter().Text(certificate.CertificateType.ToUpperInvariant()).Bold().FontSize(18).FontColor("#1b5e20");
                    column.Item().AlignCenter().Text($"Certificate No: {certificate.CertificateNumber}").FontSize(11).Bold().FontColor("#333333");

                    // Body Statement
                    column.Item().PaddingTop(24).Text(text =>
                    {
                        text.Span("This is to certify that ").FontSize(13);
                        text.Span(certificate.StudentName).Bold().FontSize(14).FontColor("#1b5e20");
                        text.Span(" bearing Admission Number ").FontSize(13);
                        text.Span(certificate.AdmissionNo).Bold().FontSize(13);
                        text.Span(" is/was a bonafide student of this institution studying in ");
                        text.Span(!string.IsNullOrWhiteSpace(certificate.AcademicLevel) ? certificate.AcademicLevel : "1st Year").Bold().FontSize(13);
                        text.Span(" (Group: ");
                        text.Span(!string.IsNullOrWhiteSpace(certificate.GroupName) ? certificate.GroupName : "General").Bold().FontSize(13);
                        text.Span(") for the Academic Year ");
                        text.Span(!string.IsNullOrWhiteSpace(certificate.AcademicYear) ? certificate.AcademicYear : $"{DateTime.UtcNow.Year}-{DateTime.UtcNow.Year + 1}").Bold().FontSize(13);
                        text.Span(".");
                    });

                    // Purpose & Remarks
                    column.Item().PaddingTop(16).Text(text =>
                    {
                        text.Span("This certificate is issued on request for the purpose of: ").FontSize(12);
                        text.Span(certificate.Purpose).Bold().FontSize(12);
                    });

                    if (!string.IsNullOrWhiteSpace(certificate.Remarks))
                    {
                        column.Item().PaddingTop(6).Text($"Remarks: {certificate.Remarks}").FontSize(11).Italic().FontColor("#666666");
                    }

                    // Metadata details table
                    column.Item().PaddingTop(20).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                        });

                        table.Cell().Text($"Request Date: {certificate.RequestDate:dd/MM/yyyy}").FontSize(11);
                        table.Cell().AlignRight().Text($"Issue Date: {certificate.IssueDate:dd/MM/yyyy}").FontSize(11);
                        table.Cell().Text($"Status: {certificate.Status}").FontSize(11).Bold();
                        table.Cell().AlignRight().Text($"Issued By: {certificate.IssuedBy ?? "Principal"}").FontSize(11);
                    });

                    // Footer with signatures
                    column.Item().PaddingTop(40).Row(row =>
                    {
                        row.RelativeItem().Column(c =>
                        {
                            c.Item().PaddingTop(30).Text("Office Seal").FontSize(11).Bold();
                        });

                        row.RelativeItem().AlignRight().Column(c =>
                        {
                            if (!string.IsNullOrWhiteSpace(signaturePath) && System.IO.File.Exists(signaturePath))
                            {
                                c.Item().Width(140).Height(50).Image(signaturePath);
                            }
                            else
                            {
                                c.Item().PaddingTop(30);
                            }
                            c.Item().Text("Principal / Authorized Signatory").FontSize(11).Bold();
                        });
                    });
                });
            });
        });

        return document.GeneratePdf();
    }
}