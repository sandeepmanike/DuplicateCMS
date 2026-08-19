using CollegeManagement.API.Data;
using CollegeManagement.API.DTOs.Reports;
using CollegeManagement.API.Models;
using CollegeManagement.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Drawing;
using System.Net;
using System.Text;
using System.Text.Json;
using static System.Net.Mime.MediaTypeNames;

namespace CollegeManagement.API.Controllers.V1;

[ApiController]
[Authorize]
[Route("api/module18-reports")]
public class Module18ReportsDetailsController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IReportService _reportService;

    public Module18ReportsDetailsController(
        AppDbContext db,
        IReportService reportService)
    {
        _db = db;
        _reportService = reportService;
    }

    // ============================================================
    // COMMON FILTER
    // ============================================================

    private static ReportFilterDto CreateFilter(
        int? boardId,
        int? academicYearId,
        int? academicLevelId,
        int? groupId,
        int? sectionId,
        DateTime? fromDate,
        DateTime? toDate)
    {
        return new ReportFilterDto
        {
            BoardId = boardId,
            AcademicYearId = academicYearId,
            AcademicLevelId = academicLevelId,
            GroupId = groupId,
            SectionId = sectionId,
            FromDate = fromDate,
            ToDate = toDate
        };
    }

    // ============================================================
    // 1. BOARDS
    // ============================================================

    [HttpGet("filters/boards")]
    public async Task<IActionResult> GetBoards(
        CancellationToken ct = default)
    {
        var result = await _db.Boards
            .AsNoTracking()
            .Where(x => x.IsActive)
            .OrderBy(x => x.BoardName)
            .Select(x => new
            {
                id = x.BoardId,
                name = x.BoardName,
                code = x.BoardCode
            })
            .ToListAsync(ct);

        return Ok(result);
    }

    // ============================================================
    // 2. ACADEMIC YEARS
    // ============================================================

    [HttpGet("filters/academic-years")]
    public async Task<IActionResult> GetAcademicYears(
        CancellationToken ct = default)
    {
        var result = await _db.AcademicYears
            .AsNoTracking()
            .Where(x => x.IsActive)
            .OrderByDescending(x => x.StartDate)
            .Select(x => new
            {
                id = x.AcademicYearId,
                name = x.AcademicYearName,
                startDate = x.StartDate,
                endDate = x.EndDate
            })
            .ToListAsync(ct);

        return Ok(result);
    }

    // ============================================================
    // 3. ACADEMIC LEVELS
    // ============================================================

    [HttpGet("filters/academic-levels")]
    public async Task<IActionResult> GetAcademicLevels(
        [FromQuery] int? boardId = null,
        CancellationToken ct = default)
    {
        IQueryable<AcademicLevel> query =
            _db.AcademicLevels
                .AsNoTracking()
                .Where(x => x.IsActive);

        if (boardId.HasValue)
        {
            query = query.Where(level =>
                _db.BoardAcademicLevels.Any(map =>
                    map.BoardId == boardId.Value &&
                    map.AcademicLevelId == level.AcademicLevelId &&
                    map.IsActive));
        }

        var result = await query
            .OrderBy(x => x.DisplayOrder)
            .ThenBy(x => x.LevelName)
            .Select(x => new
            {
                id = x.AcademicLevelId,
                name = x.LevelName,
                code = x.LevelCode
            })
            .ToListAsync(ct);

        return Ok(result);
    }

    // ============================================================
    // 4. GROUPS
    // ============================================================
    [HttpGet("filters/groups")]
    public async Task<IActionResult> GetGroups(
    [FromQuery] int? boardId = null,
    [FromQuery] int? academicYearId = null,
    [FromQuery] int? academicLevelId = null,
    CancellationToken ct = default)
    {
        var query = _db.Groups
            .AsNoTracking()
            .Where(x => x.IsActive);

        // Academic Year filter
        if (academicYearId.HasValue)
        {
            query = query.Where(x =>
                x.AcademicYearId == academicYearId.Value);
        }

        // Academic Level filter
        if (academicLevelId.HasValue)
        {
            query = query.Where(x =>
                x.AcademicLevelId == academicLevelId.Value);
        }

        // Board filter
        if (boardId.HasValue)
        {
            query = query.Where(x =>
                x.BoardId == boardId.Value
                ||
                _db.Sections.Any(s =>
                    s.IsActive &&
                    s.GroupId == x.GroupId &&
                    s.BoardId == boardId.Value)
                ||
                _db.StudentAdmissions.Any(a =>
                    a.IsActive &&
                    a.GroupId == x.GroupId &&
                    a.BoardId == boardId.Value));
        }

        var result = await query
            .OrderBy(x => x.GroupName)
            .Select(x => new
            {
                id = x.GroupId,
                name = x.GroupName,
                code = x.GroupCode,
                academicYearId = x.AcademicYearId,
                academicLevel = x.AcademicLevelNavigation != null ? x.AcademicLevelNavigation.LevelName : string.Empty,
                board = x.BoardNavigation != null ? x.BoardNavigation.BoardName : string.Empty
            })
            .ToListAsync(ct);

        return Ok(result);
    }
    // ============================================================
    // 5. SECTIONS
    // ============================================================
    [HttpGet("filters/sections")]
    public async Task<IActionResult> GetSections(
    [FromQuery] int? boardId = null,
    [FromQuery] int? academicYearId = null,
    [FromQuery] int? academicLevelId = null,
    [FromQuery] int? groupId = null,
    CancellationToken ct = default)
    {
        var query = _db.Sections
            .AsNoTracking()
            .Where(x => x.IsActive);

        // Academic Year filter
        if (academicYearId.HasValue)
        {
            query = query.Where(x =>
                x.AcademicYearId == academicYearId.Value);
        }

        // Group filter
        if (groupId.HasValue)
        {
            var groupName = await _db.Groups
                .AsNoTracking()
                .Where(x => x.GroupId == groupId.Value)
                .Select(x => x.GroupName)
                .FirstOrDefaultAsync(ct);

            query = query.Where(x =>
                x.GroupId == groupId.Value
                ||
                (!string.IsNullOrWhiteSpace(groupName)
                 && x.Group == groupName));
        }

        // Board filter
        if (boardId.HasValue)
        {
            var boardName = await _db.Boards
                .AsNoTracking()
                .Where(x => x.BoardId == boardId.Value)
                .Select(x => x.BoardName)
                .FirstOrDefaultAsync(ct);

            if (!string.IsNullOrWhiteSpace(boardName))
            {
                query = query.Where(x =>
                    x.BoardId == boardId.Value
                    ||
                    x.Board == boardName
                    ||
                    _db.StudentAdmissions.Any(a =>
                        a.IsActive &&
                        a.SectionId == x.SectionId &&
                        a.BoardId == boardId.Value));
            }
        }

        // Academic Level filter
        if (academicLevelId.HasValue)
        {
            var levelName = await _db.AcademicLevels
                .AsNoTracking()
                .Where(x => x.AcademicLevelId == academicLevelId.Value)
                .Select(x => x.LevelName)
                .FirstOrDefaultAsync(ct);

            if (!string.IsNullOrWhiteSpace(levelName))
            {
                query = query.Where(x =>
                    x.AcademicLevel == levelName);
            }
        }

        var result = await query
            .OrderBy(x => x.SectionName)
            .Select(x => new
            {
                id = x.SectionId,
                name = x.SectionName,
                groupId = x.GroupId,
                groupName = x.Group,
                boardId = x.BoardId,
                board = x.Board,
                academicYearId = x.AcademicYearId,
                academicLevel = x.AcademicLevel,
                maximumStrength = x.MaximumStrength
            })
            .ToListAsync(ct);

        return Ok(result);
    }
    // ============================================================
    // 6. ADMISSIONS DETAILS
    // ============================================================
    [HttpGet("details/admissions")]
    public async Task<IActionResult> AdmissionsDetails(
    [FromQuery] int? boardId = null,
    [FromQuery] int? academicYearId = null,
    [FromQuery] int? academicLevelId = null,
    [FromQuery] int? groupId = null,
    [FromQuery] int? sectionId = null,
    [FromQuery] DateTime? fromDate = null,
    [FromQuery] DateTime? toDate = null,
    CancellationToken ct = default)
    {
        var query = _db.StudentAdmissions
            .AsNoTracking()
            .Where(x => x.IsActive);

        if (boardId.HasValue)
        {
            query = query.Where(x =>
                x.BoardId == boardId.Value);
        }

        if (academicYearId.HasValue)
        {
            query = query.Where(x =>
                x.AcademicYearId == academicYearId.Value);
        }

        if (groupId.HasValue)
        {
            query = query.Where(x =>
                x.GroupId == groupId.Value);
        }

        if (sectionId.HasValue)
        {
            query = query.Where(x =>
                x.SectionId == sectionId.Value);
        }

        if (fromDate.HasValue)
        {
            query = query.Where(x =>
                x.AdmissionDate >= fromDate.Value.Date);
        }

        if (toDate.HasValue)
        {
            query = query.Where(x =>
                x.AdmissionDate < toDate.Value.Date.AddDays(1));
        }

        // AcademicLevel filter is intentionally not applied here
        // because the StudentAdmissions/Students DB columns
        // do not contain AcademicLevel.

        var admissions = await query
            .OrderByDescending(x => x.AdmissionDate)
            .Select(x => new
            {
                x.AdmissionId,
                x.AdmissionNo,
                x.AdmissionDate,
                x.BoardId,
                x.AcademicYearId,
                x.GroupId,
                x.SectionId,
                x.Status
            })
            .ToListAsync(ct);

        var applications = admissions.Count;

        // Real enrolled students
        var studentsQuery = _db.Students
            .AsNoTracking()
            .Where(x => x.IsActive);

        if (boardId.HasValue)
        {
            studentsQuery = studentsQuery.Where(x =>
                x.BoardId == boardId.Value);
        }

        if (academicYearId.HasValue)
        {
            studentsQuery = studentsQuery.Where(x =>
                x.AcademicYearId == academicYearId.Value);
        }

        if (groupId.HasValue)
        {
            studentsQuery = studentsQuery.Where(x =>
                x.GroupId == groupId.Value);
        }

        if (sectionId.HasValue)
        {
            studentsQuery = studentsQuery.Where(x =>
                x.SectionId == sectionId.Value);
        }

        // Do NOT use:
        // x.AcademicLevel
        //
        // because that column does not exist in Students table.

        var enrolled = await studentsQuery.CountAsync(ct);

        var conversionRate = applications == 0
            ? 0
            : Math.Round(
                enrolled * 100m / applications,
                2);

        return Ok(new
        {
            applications,
            enrolled,
            conversionRate,
            details = admissions
        });
    }
    // ============================================================
    // 7. ATTENDANCE DETAILS
    // ============================================================

    [HttpGet("details/attendance")]
    public async Task<IActionResult> AttendanceDetails(
    [FromQuery] int? boardId = null,
    [FromQuery] int? academicYearId = null,
    [FromQuery] int? academicLevelId = null,
    [FromQuery] int? groupId = null,
    [FromQuery] int? sectionId = null,
    [FromQuery] DateTime? fromDate = null,
    [FromQuery] DateTime? toDate = null,
    CancellationToken ct = default)
    {
        var query = _db.Attendances
            .AsNoTracking()
            .Where(x => x.IsActive);

        if (boardId.HasValue)
            query = query.Where(x => x.AttendanceSession.BoardId == boardId.Value);

        if (academicYearId.HasValue)
            query = query.Where(x =>
                x.AttendanceSession.AcademicYearId == academicYearId.Value);

        if (academicLevelId.HasValue)
            query = query.Where(x =>
                x.AttendanceSession.AcademicLevelId == academicLevelId.Value);

        if (groupId.HasValue)
            query = query.Where(x =>
                x.AttendanceSession.GroupId == groupId.Value);

        if (sectionId.HasValue)
            query = query.Where(x =>
                x.AttendanceSession.SectionId == sectionId.Value);

        // IMPORTANT:
        // Date filtering only when frontend sends dates.
        if (fromDate.HasValue)
        {
            query = query.Where(x =>
                x.AttendanceSession.AttendanceDate >= fromDate.Value.Date);
        }

        if (toDate.HasValue)
        {
            var end = toDate.Value.Date.AddDays(1);

            query = query.Where(x =>
                x.AttendanceSession.AttendanceDate < end);
        }

        var records = await query
            .OrderByDescending(x => x.AttendanceSession.AttendanceDate)
            .Select(x => new
            {
                x.AttendanceId,
                x.StudentId,
                x.AttendanceSession.FacultyId,
                x.AttendanceSession.BoardId,
                x.AttendanceSession.AcademicYearId,
                x.AttendanceSession.AcademicLevelId,
                x.AttendanceSession.GroupId,
                x.AttendanceSession.SectionId,
                x.AttendanceSession.SubjectId,
                x.AttendanceSession.AttendanceDate,
                x.Status,
                x.Remarks
            })
            .ToListAsync(ct);

        var total = records.Count;

        var present = records.Count(x => (int)x.Status == 1);
        var absent = records.Count(x => (int)x.Status == 2);
        var late = records.Count(x => (int)x.Status == 3);
        var leave = records.Count(x => (int)x.Status == 4);

        var percentage = total == 0
            ? 0
            : Math.Round(present * 100m / total, 2);

        return Ok(new
        {
            total,
            present,
            absent,
            late,
            leave,
            attendancePercentage = percentage,
            records
        });
    }

    // ============================================================
    // 8. FEE COLLECTION DETAILS
    // ============================================================

    [HttpGet("details/fee-collection")]
    public async Task<IActionResult> FeeCollectionDetails(
    [FromQuery] int? boardId = null,
    [FromQuery] int? academicYearId = null,
    [FromQuery] int? academicLevelId = null,
    [FromQuery] int? groupId = null,
    [FromQuery] int? sectionId = null,
    [FromQuery] DateTime? fromDate = null,
    [FromQuery] DateTime? toDate = null,
    CancellationToken ct = default)
    {
        var query = _db.Students
            .AsNoTracking()
            .Where(x => x.IsActive && x.FeePaid > 0);

        if (boardId.HasValue)
            query = query.Where(x =>
                x.BoardId == boardId.Value);

        if (academicYearId.HasValue)
            query = query.Where(x =>
                x.AcademicYearId == academicYearId.Value);

        if (academicLevelId.HasValue)
            query = query.Where(x =>
                x.AcademicLevelId == academicLevelId.Value);

        if (groupId.HasValue)
            query = query.Where(x =>
                x.GroupId == groupId.Value);

        if (sectionId.HasValue)
            query = query.Where(x =>
                x.SectionId == sectionId.Value);

        if (fromDate.HasValue)
            query = query.Where(x =>
                (x.UpdatedAt ?? x.CreatedAt) >= fromDate.Value.Date);

        if (toDate.HasValue)
            query = query.Where(x =>
                (x.UpdatedAt ?? x.CreatedAt) < toDate.Value.Date.AddDays(1));

        var transactions = await query
            .OrderByDescending(x => x.UpdatedAt ?? x.CreatedAt)
            .Select(x => new
            {
                FeeCollectionId = x.StudentId,
                StudentId = x.StudentId,
                StudentName = x.StudentName,
                AdmissionNo = x.AdmissionNo,
                RollNo = x.RollNo,
                PaymentDate = x.UpdatedAt ?? x.CreatedAt,
                PaidAmount = x.FeePaid,
                DueAmount = x.FeeAmount - x.FeePaid > 0 ? x.FeeAmount - x.FeePaid : 0m,
                Discount = 0m,
                Fine = 0m
            })
            .ToListAsync(ct);

        var collected = transactions.Sum(x => x.PaidAmount);
        var pending = transactions.Sum(x => x.DueAmount);
        var discount = transactions.Sum(x => x.Discount);
        var fine = transactions.Sum(x => x.Fine);

        var rate = collected + pending == 0
            ? 0
            : Math.Round(
                collected * 100m / (collected + pending),
                2);

        return Ok(new
        {
            collected,
            pending,
            discount,
            fine,
            transactions = transactions.Count,
            collectionRate = rate,
            details = transactions
        });
    }
    // ============================================================
    // 9. DUE FEES DETAILS
    // ============================================================

    [HttpGet("details/due-fees")]
    public async Task<IActionResult> DueFeesDetails(
        [FromQuery] int? boardId = null,
        [FromQuery] int? academicYearId = null,
        [FromQuery] int? academicLevelId = null,
        [FromQuery] int? groupId = null,
        [FromQuery] int? sectionId = null,
        CancellationToken ct = default)
    {
        var query = _db.StudentFees
            .AsNoTracking()
            .Where(x => x.DueAmount > 0);

        if (boardId.HasValue)
        {
            query = query.Where(x =>
                _db.Students.Any(s =>
                    s.StudentId == x.StudentId &&
                    s.BoardId == boardId.Value));
        }

        if (academicYearId.HasValue)
        {
            query = query.Where(x =>
                _db.Students.Any(s =>
                    s.StudentId == x.StudentId &&
                    s.AcademicYearId ==
                    academicYearId.Value));
        }

        if (groupId.HasValue)
        {
            query = query.Where(x =>
                _db.Students.Any(s =>
                    s.StudentId == x.StudentId &&
                    s.GroupId == groupId.Value));
        }

        if (sectionId.HasValue)
        {
            query = query.Where(x =>
                _db.Students.Any(s =>
                    s.StudentId == x.StudentId &&
                    s.SectionId == sectionId.Value));
        }

        if (academicLevelId.HasValue)
        {
            query = query.Where(x =>
                _db.Students.Any(s =>
                    s.StudentId == x.StudentId &&
                    s.AcademicLevelId == academicLevelId.Value));
        }

        var totalOutstanding =
            await query.SumAsync(
                x => (decimal?)x.DueAmount,
                ct) ?? 0;

        var studentsDue =
            await query
                .Select(x => x.StudentId)
                .Distinct()
                .CountAsync(ct);

        var details = await query
            .Join(
                _db.Students,
                fee => fee.StudentId,
                student => student.StudentId,
                (fee, student) => new
                {
                    student.StudentId,
                    student.AdmissionNo,
                    student.RollNo,
                    student.StudentName,
                    fee.DueAmount
                })
            .OrderByDescending(x => x.DueAmount)
            .ToListAsync(ct);

        return Ok(new
        {
            totalOutstanding,
            studentsDue,

            // No DueDate exists in StudentFee,
            // so 30+ days cannot be calculated exactly.
            overdue30Plus = (decimal?)null,

            details
        });
    }

    // ============================================================
    // 10. EXAMINATION DETAILS
    // ============================================================

    [HttpGet("details/examinations")]
    public async Task<IActionResult> ExaminationDetails(
        [FromQuery] int? boardId = null,
        [FromQuery] int? academicYearId = null,
        [FromQuery] int? academicLevelId = null,
        [FromQuery] int? groupId = null,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null,
        CancellationToken ct = default)
    {
        var query = _db.Examinations
            .AsNoTracking()
            .Where(x => x.IsActive);

        if (boardId.HasValue)
            query = query.Where(x =>
                x.BoardId == boardId.Value);

        if (academicYearId.HasValue)
            query = query.Where(x =>
                x.AcademicYearId ==
                academicYearId.Value);

        if (academicLevelId.HasValue)
            query = query.Where(x =>
                x.AcademicLevelId ==
                academicLevelId.Value);

        if (groupId.HasValue)
            query = query.Where(x =>
                x.GroupId == groupId.Value);

        if (fromDate.HasValue)
        {
            var date =
                DateOnly.FromDateTime(
                    fromDate.Value.Date);

            query = query.Where(x =>
                x.EndDate >= date);
        }

        if (toDate.HasValue)
        {
            var date =
                DateOnly.FromDateTime(
                    toDate.Value.Date);

            query = query.Where(x =>
                x.StartDate <= date);
        }

        var exams = await query
            .Select(x => new
            {
                x.ExaminationId,
                x.ExamName,
                x.StartDate,
                x.EndDate,
                x.Status
            })
            .ToListAsync(ct);

        var today =
            DateOnly.FromDateTime(
                DateTime.Today);

        var upcoming =
            exams.Count(x =>
                x.StartDate > today);

        var ongoing =
            exams.Count(x =>
                x.StartDate <= today &&
                x.EndDate >= today);

        var completed =
            exams.Count(x =>
                x.EndDate < today);

        return Ok(new
        {
            upcoming,
            ongoing,
            completed,
            total = exams.Count,
            exams
        });
    }

    // ============================================================
    // 11. RESULTS DETAILS
    // ============================================================
    [HttpGet("details/results")]
    public async Task<IActionResult> ResultDetails(
        [FromQuery] int? boardId = null,
        [FromQuery] int? academicYearId = null,
        [FromQuery] int? academicLevelId = null,
        [FromQuery] int? groupId = null,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null,
        CancellationToken ct = default)
    {
        var query = _db.Results
            .AsNoTracking();

        if (boardId.HasValue)
            query = query.Where(x =>
                x.BoardId == boardId.Value);

        if (academicYearId.HasValue)
            query = query.Where(x =>
                x.AcademicYearId == academicYearId.Value);

        if (academicLevelId.HasValue)
            query = query.Where(x =>
                x.AcademicLevelId == academicLevelId.Value);

        if (groupId.HasValue)
            query = query.Where(x =>
                x.GroupId == groupId.Value);

        if (fromDate.HasValue)
            query = query.Where(x =>
                x.PublishedDate >= fromDate.Value);

        if (toDate.HasValue)
            query = query.Where(x =>
                x.PublishedDate < toDate.Value.Date.AddDays(1));

        var records = await query
            .OrderByDescending(x => x.TotalMarks)
            .Select(x => new
            {
                x.ResultId,
                x.StudentId,
                studentName = x.Student.StudentName,
                x.SubjectId,
                subjectName = x.Subject.SubjectName,
                x.ExamId,
                x.InternalMarks,
                x.PracticalMarks,
                x.ExternalMarks,
                x.TotalMarks,
                x.Grade,
                x.ResultStatus,
                x.Rank,
                x.IsPublished,
                x.PublishedDate
            })
            .ToListAsync(ct);

        var published = records.Count(x => x.IsPublished);
        var pending = records.Count(x => !x.IsPublished);

        var passed = records.Count(x =>
            x.ResultStatus != null &&
            (x.ResultStatus.ToUpper() == "PASS" ||
             x.ResultStatus.ToUpper() == "PASSED"));

        var failed = records.Count(x =>
            x.ResultStatus != null &&
            (x.ResultStatus.ToUpper() == "FAIL" ||
             x.ResultStatus.ToUpper() == "FAILED"));

        return Ok(new
        {
            published,
            pending,
            passed,
            failed,
            total = records.Count,
            results = records
        });
    }
    // ============================================================
    // 12. FACULTY WORKLOAD
    // ============================================================

    [HttpGet("details/faculty-workload")]
    public async Task<IActionResult> FacultyWorkloadDetails(
        [FromQuery] ReportFilterDto filter,
        CancellationToken ct = default)
    {
        var data =
            await _reportService.FacultyWorkloadAsync(
                filter,
                ct);

        var totalFaculty =
            data.Select(x => x.FacultyId)
                .Distinct()
                .Count();

        var totalHours =
            data.Sum(x => x.HoursPerWeek);

        var averageHours =
            totalFaculty == 0
                ? 0
                : Math.Round(
                    totalHours /
                    totalFaculty,
                    2);

        return Ok(new
        {
            totalFaculty,
            totalHours,
            averageHoursPerFaculty =
                averageHours,
            details = data
        });
    }

    // ============================================================
    // 13. STUDENT STRENGTH
    // ============================================================

    [HttpGet("details/student-strength")]
    public async Task<IActionResult> StudentStrengthDetails(
        [FromQuery] int? boardId = null,
        [FromQuery] int? academicYearId = null,
        [FromQuery] int? academicLevelId = null,
        [FromQuery] int? groupId = null,
        [FromQuery] int? sectionId = null,
        CancellationToken ct = default)
    {
        var query = _db.Students
            .AsNoTracking()
            .Where(x => x.IsActive);

        if (boardId.HasValue)
            query = query.Where(x =>
                x.BoardId == boardId.Value);

        if (academicYearId.HasValue)
            query = query.Where(x =>
                x.AcademicYearId ==
                academicYearId.Value);

        if (academicLevelId.HasValue)
        {
            query = query.Where(x =>
                x.AcademicLevelId == academicLevelId.Value);
        }

        if (groupId.HasValue)
            query = query.Where(x =>
                x.GroupId == groupId.Value);

        if (sectionId.HasValue)
            query = query.Where(x =>
                x.SectionId == sectionId.Value);

        var total =
            await query.CountAsync(ct);

        var male =
            await query.CountAsync(
                x => x.Gender.ToUpper() == "MALE",
                ct);

        var female =
            await query.CountAsync(
                x => x.Gender.ToUpper() == "FEMALE",
                ct);

        var other =
            total - male - female;

        return Ok(new
        {
            totalStudents = total,
            maleStudents = male,
            femaleStudents = female,
            otherStudents = other
        });
    }

    // ============================================================
    // 14. PASS PERCENTAGE
    // ============================================================

    [HttpGet("details/pass-percentage")]
    public async Task<IActionResult> PassPercentageDetails(
        [FromQuery] ReportFilterDto filter,
        CancellationToken ct = default)
    {
        var data =
            await _reportService.PassPercentageAsync(
                filter,
                ct);

        var passed =
            data.Sum(x => x.Passed);

        var failed =
            data.Sum(x => x.Failed);

        var appeared =
            passed + failed;

        var percentage =
            appeared == 0
                ? 0
                : Math.Round(
                    passed * 100m /
                    appeared,
                    2);

        return Ok(new
        {
            appeared,
            passed,
            failed,
            passPercentage = percentage,
            details = data
        });
    }

    // ============================================================
    // 15. TOPPERS
    // ============================================================
    [HttpGet("details/toppers")]
    public async Task<IActionResult> TopperDetails(
    [FromQuery] int? boardId = null,
    [FromQuery] int? academicYearId = null,
    [FromQuery] int? academicLevelId = null,
    [FromQuery] int? groupId = null,
    [FromQuery] DateTime? fromDate = null,
    [FromQuery] DateTime? toDate = null,
    CancellationToken ct = default)
    {
        var query = _db.Results
            .AsNoTracking()
            .Where(x => x.IsPublished);

        if (boardId.HasValue)
            query = query.Where(x =>
                x.BoardId == boardId.Value);

        if (academicYearId.HasValue)
            query = query.Where(x =>
                x.AcademicYearId == academicYearId.Value);

        if (academicLevelId.HasValue)
            query = query.Where(x =>
                x.AcademicLevelId == academicLevelId.Value);

        if (groupId.HasValue)
            query = query.Where(x =>
                x.GroupId == groupId.Value);

        if (fromDate.HasValue)
            query = query.Where(x =>
                x.PublishedDate >= fromDate.Value);

        if (toDate.HasValue)
            query = query.Where(x =>
                x.PublishedDate < toDate.Value.Date.AddDays(1));

        var topperData = await query
            .GroupBy(x => new
            {
                x.StudentId,
                StudentName = x.Student.StudentName,
                x.GroupId
            })
            .Select(g => new
            {
                studentId = g.Key.StudentId,
                studentName = g.Key.StudentName,
                groupId = g.Key.GroupId,

                subjects = g.Count(),

                totalMarks = g.Sum(x => x.TotalMarks),

                averagePercentage =
                    g.Average(x => x.TotalMarks),

                passedSubjects = g.Count(x =>
                    x.ResultStatus != null &&
                    x.ResultStatus.ToUpper() == "PASS"),

                failedSubjects = g.Count(x =>
                    x.ResultStatus != null &&
                    x.ResultStatus.ToUpper() == "FAIL")
            })
            .OrderByDescending(x => x.averagePercentage)
            .ThenByDescending(x => x.totalMarks)
            .ToListAsync(ct);

        var toppers = topperData
            .Select((x, index) => new
            {
                rank = index + 1,
                x.studentId,
                x.studentName,
                x.groupId,
                x.subjects,
                x.totalMarks,
                percentage = Math.Round(
                    x.averagePercentage, 2),
                x.passedSubjects,
                x.failedSubjects
            })
            .ToList();

        return Ok(new
        {
            identified = toppers.Count,
            universityTopper = toppers.FirstOrDefault(),
            toppers
        });
    }
    // ============================================================
    // 16. AUDIT LOGS
    // ============================================================

    [HttpGet("audit-logs")]
    public async Task<IActionResult> AuditLogs(
        [FromQuery] ReportFilterDto filter,
        CancellationToken ct = default)
    {
        var data =
            await _reportService.AuditLogsAsync(
                filter,
                ct);

        return Ok(new
        {
            total = data.Count,
            records = data
        });
    }


    // ============================================================
    // 17. CUSTOM REPORT
    // ============================================================

    [HttpPost("custom")]
    public async Task<IActionResult> CustomReport(
        [FromBody] CustomReportRequestDto request,
        CancellationToken ct = default)
    {
        if (request == null)
            return BadRequest("Request body is required.");

        if (string.IsNullOrWhiteSpace(request.ReportType))
            return BadRequest("ReportType is required.");

        var data = await _reportService.CustomAsync(request, ct);

        return Ok(data);
    }

    // ============================================================
    // 18. GENERATE REPORT
    // ============================================================

    [HttpGet("generate/{reportType}")]
    public async Task<IActionResult> GenerateReport(
        string reportType,
        [FromQuery] ReportFilterDto filter,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(reportType))
            return BadRequest("Report type is required.");

        var request = new CustomReportRequestDto
        {
            ReportType = reportType,
            BoardId = filter.BoardId,
            AcademicYearId = filter.AcademicYearId,
            AcademicLevelId = filter.AcademicLevelId,
            GroupId = filter.GroupId,
            SectionId = filter.SectionId,
            FromDate = filter.FromDate,
            ToDate = filter.ToDate
        };

        var data = await _reportService.CustomAsync(request, ct);

        return Ok(new
        {
            success = true,
            reportType = reportType,
            generatedAt = DateTime.Now,
            filters = filter,
            data = data
        });
    }

    // ============================================================
    // 19. PRINT REPORT
    // ============================================================
    // ============================================================
    // 19. PRINT REPORT
    // ============================================================

    [HttpGet("print/{reportType}")]
    public async Task<IActionResult> PrintReport(
        string reportType,
        [FromQuery] ReportFilterDto filter,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(reportType))
            return BadRequest("Report type is required.");

        var result = await _reportService.ExportAsync(
            reportType,
            filter,
            pdf: true,
            ct);

        Response.Headers["Content-Disposition"] =
            $"inline; filename=\"{result.FileName}\"";

        return File(
            result.Content,
            "application/pdf");
    }
    // ============================================================
    // 19. EXCEL EXPORT
    // ============================================================

    [HttpGet("export/excel/{reportType}")]
    public async Task<IActionResult> ExportExcel(
        string reportType,
        [FromQuery] ReportFilterDto filter,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(reportType))
        {
            return BadRequest("Report type is required.");
        }

        var result = await _reportService.ExportAsync(
            reportType,
            filter,
            pdf: false,
            ct);

        return File(
            result.Content,
            result.ContentType,
            result.FileName);
    }


    // ============================================================
    // 20. PDF EXPORT
    // ============================================================

    [HttpGet("export/pdf/{reportType}")]
    public async Task<IActionResult> ExportPdf(
        string reportType,
        [FromQuery] ReportFilterDto filter,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(reportType))
        {
            return BadRequest("Report type is required.");
        }

        var result = await _reportService.ExportAsync(
            reportType,
            filter,
            pdf: true,
            ct);

        return File(
            result.Content,
            result.ContentType,
            result.FileName);
    }
}