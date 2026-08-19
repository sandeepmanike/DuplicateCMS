using CollegeManagement.API.DTOs.Reports;
using CollegeManagement.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CollegeManagement.API.Controllers.V1;

[ApiController]
[Authorize]
[Route("api/reports")]
public class ReportsController : ControllerBase
{
    private readonly IReportService _service;
    public ReportsController(IReportService service) => _service = service;

    [HttpGet("dashboard")]
    public async Task<IActionResult> Dashboard([FromQuery] ReportFilterDto filter, CancellationToken ct = default)
        => Ok(await _service.DashboardAsync(filter, ct));

    [HttpGet("summary")]
    public async Task<IActionResult> Summary([FromQuery] ReportFilterDto filter, CancellationToken ct = default)
        => Ok(await _service.DashboardAsync(filter, ct));

    [HttpGet("admissions")]
    public async Task<IActionResult> Admissions([FromQuery] ReportFilterDto filter, CancellationToken ct = default)
        => Ok(await _service.AdmissionsAsync(filter, ct));

    [HttpGet("student-strength")]
    public async Task<IActionResult> StudentStrength([FromQuery] ReportFilterDto filter, CancellationToken ct = default)
        => Ok(await _service.StudentStrengthAsync(filter, ct));

    [HttpGet("attendance")]
    public async Task<IActionResult> Attendance([FromQuery] ReportFilterDto filter, CancellationToken ct = default)
        => Ok(await _service.AttendanceAsync(filter, ct));

    [HttpGet("faculty-attendance")]
    public async Task<IActionResult> FacultyAttendance([FromQuery] ReportFilterDto filter, CancellationToken ct = default)
        => Ok(await _service.FacultyAttendanceAsync(filter, ct));

    [HttpGet("fees/collection")]
    public async Task<IActionResult> FeeCollection([FromQuery] ReportFilterDto filter, CancellationToken ct = default)
        => Ok(await _service.FeeCollectionAsync(filter, ct));

    [HttpGet("fees/outstanding")]
    public async Task<IActionResult> OutstandingFees([FromQuery] ReportFilterDto filter, CancellationToken ct = default)
        => Ok(await _service.OutstandingFeesAsync(filter, ct));

    [HttpGet("examinations")]
    public async Task<IActionResult> Examinations([FromQuery] ReportFilterDto filter, CancellationToken ct = default)
        => Ok(await _service.ExaminationsAsync(filter, ct));

    [HttpGet("results")]
    public async Task<IActionResult> Results([FromQuery] ReportFilterDto filter, CancellationToken ct = default)
        => Ok(await _service.ResultsAsync(filter, ct));

    [HttpGet("pass-percentage")]
    public async Task<IActionResult> PassPercentage([FromQuery] ReportFilterDto filter, CancellationToken ct = default)
        => Ok(await _service.PassPercentageAsync(filter, ct));

    [HttpGet("toppers")]
    public async Task<IActionResult> Toppers([FromQuery] ReportFilterDto filter, CancellationToken ct = default)
        => Ok(await _service.ToppersAsync(filter, ct));

    [HttpGet("subjects")]
    public async Task<IActionResult> Subjects([FromQuery] ReportFilterDto filter, CancellationToken ct = default)
        => Ok(await _service.SubjectsAsync(filter, ct));

    [HttpGet("groups")]
    public async Task<IActionResult> Groups([FromQuery] ReportFilterDto filter, CancellationToken ct = default)
        => Ok(await _service.GroupsAsync(filter, ct));

    [HttpGet("sections")]
    public async Task<IActionResult> Sections([FromQuery] ReportFilterDto filter, CancellationToken ct = default)
        => Ok(await _service.SectionsAsync(filter, ct));

    [HttpGet("faculty-workload")]
    public async Task<IActionResult> FacultyWorkload([FromQuery] ReportFilterDto filter, CancellationToken ct = default)
        => Ok(await _service.FacultyWorkloadAsync(filter, ct));

    [HttpGet("student-performance")]
    public async Task<IActionResult> StudentPerformance([FromQuery] ReportFilterDto filter, CancellationToken ct = default)
        => Ok(await _service.StudentPerformanceAsync(filter, ct));

    [HttpGet("audit-logs")]
    public async Task<IActionResult> AuditLogs([FromQuery] ReportFilterDto filter, CancellationToken ct = default)
        => Ok(await _service.AuditLogsAsync(filter, ct));

    [HttpGet("export/pdf")]
    public async Task<IActionResult> ExportPdf([FromQuery] string reportType = "dashboard", [FromQuery] ReportFilterDto? filter = null, CancellationToken ct = default)
    {
        var result = await _service.ExportAsync(reportType, filter ?? new ReportFilterDto(), true, ct);
        return File(result.Content, result.ContentType, result.FileName);
    }

    [HttpGet("export/excel")]
    public async Task<IActionResult> ExportExcel([FromQuery] string reportType = "dashboard", [FromQuery] ReportFilterDto? filter = null, CancellationToken ct = default)
    {
        var result = await _service.ExportAsync(reportType, filter ?? new ReportFilterDto(), false, ct);
        return File(result.Content, result.ContentType, result.FileName);
    }

    [HttpPost("custom")]
    public async Task<IActionResult> Custom([FromBody] CustomReportRequestDto request, CancellationToken ct = default)
        => Ok(await _service.CustomAsync(request, ct));
}
