using System.Globalization;
using System.Text;
using CollegeManagement.API.DTOs.Reports;
using CollegeManagement.API.Models.Reports;
using CollegeManagement.API.Repositories.Interfaces;
using CollegeManagement.API.Services.Interfaces;
using ClosedXML.Excel;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace CollegeManagement.API.Services.Implementations;

public class ReportService : IReportService
{
    private readonly IReportRepository _repo;

    public ReportService(IReportRepository repo)
    {
        _repo = repo;
    }

    private static ReportFilterModel M(ReportFilterDto f) => new()
    {
        BoardId = f.BoardId,
        AcademicYearId = f.AcademicYearId,
        AcademicLevelId = f.AcademicLevelId,
        GroupId = f.GroupId,
        SectionId = f.SectionId,
        FromDate = f.FromDate,
        ToDate = f.ToDate
    };

    // ---------- Generic Safe Helpers ----------
    private static async Task<T> SafeAsync<T>(Func<Task<T>> action, Func<T> fallback)
    {
        try
        {
            return await action();
        }
        catch
        {
            return fallback();
        }
    }
    private static async Task<IReadOnlyList<T>> SafeListAsync<T>(Func<Task<IReadOnlyList<T>>> action)
    {
        try
        {
            return await action();
        }
        catch
        {
            return Array.Empty<T>();
        }
    }

    // ---------- Dashboard ----------
    public Task<DashboardReportDto> DashboardAsync(ReportFilterDto f, CancellationToken ct = default)
        => SafeAsync(() => _repo.GetDashboardAsync(M(f), ct), () => new DashboardReportDto());

    // ---------- Admissions ----------
    public Task<IReadOnlyList<AdmissionReportDto>> AdmissionsAsync(ReportFilterDto f, CancellationToken ct = default)
        => SafeListAsync(() => _repo.GetAdmissionsAsync(M(f), ct));

    // ---------- Student Strength ----------
    public Task<IReadOnlyList<StudentStrengthReportDto>> StudentStrengthAsync(ReportFilterDto f, CancellationToken ct = default)
        => SafeListAsync(() => _repo.GetStudentStrengthAsync(M(f), ct));

    // ---------- Attendance ----------
    public Task<IReadOnlyList<AttendanceReportDto>> AttendanceAsync(ReportFilterDto f, CancellationToken ct = default)
        => SafeListAsync(() => _repo.GetAttendanceAsync(M(f), ct));

    // ---------- Faculty Attendance ----------
    public Task<IReadOnlyList<FacultyAttendanceReportDto>> FacultyAttendanceAsync(ReportFilterDto f, CancellationToken ct = default)
        => SafeListAsync(() => _repo.GetFacultyAttendanceAsync(M(f), ct));

    // ---------- Fee Collection ----------
    public Task<IReadOnlyList<FeeCollectionReportDto>> FeeCollectionAsync(ReportFilterDto f, CancellationToken ct = default)
        => SafeListAsync(() => _repo.GetFeeCollectionAsync(M(f), ct));

    // ---------- Outstanding Fees ----------
    public Task<IReadOnlyList<OutstandingFeeReportDto>> OutstandingFeesAsync(ReportFilterDto f, CancellationToken ct = default)
        => SafeListAsync(() => _repo.GetOutstandingFeesAsync(M(f), ct));

    // ---------- Examinations ----------
    public Task<IReadOnlyList<ExaminationReportDto>> ExaminationsAsync(ReportFilterDto f, CancellationToken ct = default)
        => SafeListAsync(() => _repo.GetExaminationsAsync(M(f), ct));

    // ---------- Results ----------
    public Task<IReadOnlyList<ResultAnalysisReportDto>> ResultsAsync(ReportFilterDto f, CancellationToken ct = default)
        => SafeListAsync(() => _repo.GetResultsAsync(M(f), ct));

    // ---------- Pass Percentage ----------
    public Task<IReadOnlyList<PassPercentageReportDto>> PassPercentageAsync(ReportFilterDto f, CancellationToken ct = default)
        => SafeListAsync(() => _repo.GetPassPercentageAsync(M(f), ct));

    // ---------- Toppers ----------
    public Task<IReadOnlyList<TopperReportDto>> ToppersAsync(ReportFilterDto f, CancellationToken ct = default)
        => SafeListAsync(() => _repo.GetToppersAsync(M(f), ct));

    // ---------- Subjects ----------
    public Task<IReadOnlyList<SubjectWiseReportDto>> SubjectsAsync(ReportFilterDto f, CancellationToken ct = default)
        => SafeListAsync(() => _repo.GetSubjectsAsync(M(f), ct));

    // ---------- Groups ----------
    public Task<IReadOnlyList<GroupWiseReportDto>> GroupsAsync(ReportFilterDto f, CancellationToken ct = default)
        => SafeListAsync(() => _repo.GetGroupsAsync(M(f), ct));

    // ---------- Sections ----------
    public Task<IReadOnlyList<SectionWiseReportDto>> SectionsAsync(ReportFilterDto f, CancellationToken ct = default)
        => SafeListAsync(() => _repo.GetSectionsAsync(M(f), ct));

    // ---------- Faculty Workload ----------
    public Task<IReadOnlyList<FacultyWorkloadReportDto>> FacultyWorkloadAsync(ReportFilterDto f, CancellationToken ct = default)
        => SafeListAsync(() => _repo.GetFacultyWorkloadAsync(M(f), ct));

    // ---------- Student Performance ----------
    public Task<IReadOnlyList<StudentPerformanceReportDto>> StudentPerformanceAsync(ReportFilterDto f, CancellationToken ct = default)
        => SafeListAsync(() => _repo.GetStudentPerformanceAsync(M(f), ct));

    // ---------- Audit Logs ----------
    public Task<IReadOnlyList<AuditLogDto>> AuditLogsAsync(ReportFilterDto f, CancellationToken ct = default)
        => SafeListAsync(() => _repo.GetAuditLogsAsync(M(f), ct));

    // ---------- Custom Report ----------
    public async Task<object> CustomAsync(
    CustomReportRequestDto request,
    CancellationToken ct = default)
    {
        return request.ReportType.Trim().ToLowerInvariant() switch
        {
            "dashboard" or "summary" => await DashboardAsync(request, ct),
            "admissions" or "admission" => await AdmissionsAsync(request, ct),
            "student-strength" or "studentstrength" => await StudentStrengthAsync(request, ct),
            "attendance" => await AttendanceAsync(request, ct),
            "faculty-attendance" => await FacultyAttendanceAsync(request, ct),
            "fees" or "fee-collection" => await FeeCollectionAsync(request, ct),
            "outstanding" or "outstanding-fees" => await OutstandingFeesAsync(request, ct),
            "examinations" or "exams" => await ExaminationsAsync(request, ct),
            "results" => await ResultsAsync(request, ct),
            "pass-percentage" or "pass" => await PassPercentageAsync(request, ct),
            "toppers" => await ToppersAsync(request, ct),
            "subjects" or "subject-wise" => await SubjectsAsync(request, ct),
            "groups" or "group-wise" => await GroupsAsync(request, ct),
            "sections" or "section-wise" => await SectionsAsync(request, ct),
            "faculty-workload" => await FacultyWorkloadAsync(request, ct),
            "student-performance" => await StudentPerformanceAsync(request, ct),
            "audit-logs" or "audit" => await AuditLogsAsync(request, ct),
            _ => new { Message = "Unsupported report type" }
        };
    }

    // ---------- Export PDF / Excel ----------
    public async Task<(byte[] Content, string ContentType, string FileName)> ExportAsync(
     string reportType,
     ReportFilterDto filter,
     bool pdf,
     CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(reportType))
            throw new ArgumentException("Report type is required.");

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

        // Fetch REAL database data through repository
        var data = await CustomAsync(request, ct);

        var json = System.Text.Json.JsonSerializer.Serialize(
            data,
            new System.Text.Json.JsonSerializerOptions
            {
                WriteIndented = true
            });

        // Excel/JSON export
        // Excel export - REAL DATABASE DATA
        if (!pdf)
        {
            using var workbook = new XLWorkbook();

            var sheetName = string.IsNullOrWhiteSpace(reportType)
                ? "Report"
                : reportType.Trim();

            if (sheetName.Length > 31)
                sheetName = sheetName.Substring(0, 31);

            var worksheet = workbook.Worksheets.Add(sheetName);

            worksheet.Cell(1, 1).Value = "Report";
            worksheet.Cell(1, 2).Value = reportType;

            worksheet.Cell(2, 1).Value = "Generated";
            worksheet.Cell(2, 2).Value =
                DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");

            worksheet.Cell(4, 1).Value = "Report Data";

            worksheet.Cell(5, 1).Value = json;

            worksheet.Column(1).Width = 30;
            worksheet.Column(2).Width = 80;

            worksheet.Row(1).Style.Font.Bold = true;
            worksheet.Row(2).Style.Font.Bold = true;
            worksheet.Row(4).Style.Font.Bold = true;

            worksheet.Cell(4, 1).Style.Font.Bold = true;

            using var stream = new MemoryStream();

            workbook.SaveAs(stream);

            return (
                stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"{reportType}-report.xlsx"
            );
        }

        // PDF export
        QuestPDF.Settings.License = LicenseType.Community;

        var pdfBytes = Document.Create(document =>
        {
            document.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(35);

                page.Header()
                    .Text($"{reportType.ToUpperInvariant()} REPORT")
                    .FontSize(20)
                    .Bold();

                page.Content()
                    .PaddingTop(20)
                    .Column(column =>
                    {
                        column.Item()
                            .Text($"Generated: {DateTime.Now:dd-MM-yyyy HH:mm:ss}")
                            .FontSize(10);

                        column.Item()
                            .PaddingTop(20)
                            .Text("Report Data")
                            .FontSize(14)
                            .Bold();

                        column.Item()
                            .PaddingTop(10)
                            .Background(Colors.Grey.Lighten4)
                            .Padding(15)
                            .Text(json)
                            .FontSize(9);
                    });

                page.Footer()
                    .AlignCenter()
                    .Text("College Management System")
                    .FontSize(8);
            });
        }).GeneratePdf();

        return (
            pdfBytes,
            "application/pdf",
            $"{reportType}-report.pdf"
        );
    }
}