using CollegeManagement.API.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CollegeManagement.API.Controllers.V1;

[ApiController]
[Route("api/v1/dashboard")]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly AppDbContext _db;
    public DashboardController(AppDbContext db) => _db = db;

    [HttpGet("summary")]
    public async Task<IActionResult> Summary(CancellationToken ct = default)
    {
        var today = DateTime.UtcNow.Date;
        var totalStudents = await _db.Students.CountAsync(x => x.IsActive, ct);
        var totalFaculty = await _db.Faculties.CountAsync(x => !x.IsDeleted && x.Status == "Active", ct);
        var totalGroups = await _db.Groups.CountAsync(x => x.IsActive, ct);
        var totalSubjects = await _db.Subjects.CountAsync(x => x.IsActive, ct);
        var totalSections = await _db.Sections.CountAsync(x => x.IsActive, ct);
        var pendingAdmissions = await _db.StudentAdmissions.CountAsync(x => !x.IsApproved && !x.IsRejected && x.IsActive, ct);
        var todayAdmissions = await _db.StudentAdmissions.CountAsync(x => x.AdmissionDate.Date == today, ct);
        var todayAttendance = await _db.Attendances.CountAsync(x => x.AttendanceSession.AttendanceDate.Date == today, ct);

        return Ok(new
        {
            totalStudents,
            totalFaculty,
            totalGroups,
            totalSubjects,
            totalSections,
            pendingAdmissions,
            todayAdmissions,
            todayAttendance
        });
    }

    [HttpGet("admission-trend")]
    public async Task<IActionResult> AdmissionTrend([FromQuery] int? academicYearId = null, CancellationToken ct = default)
    {
        var query = _db.StudentAdmissions.AsNoTracking().AsQueryable();
        if (academicYearId.HasValue) query = query.Where(x => x.AcademicYearId == academicYearId.Value);
        var rows = await query.GroupBy(x => new { x.AdmissionDate.Year, x.AdmissionDate.Month })
            .Select(g => new { year = g.Key.Year, month = g.Key.Month, admissions = g.Count() })
            .OrderBy(x => x.year).ThenBy(x => x.month).ToListAsync(ct);
        return Ok(rows);
    }
}
