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

        // Convert structured DTOs to Tabular Grid
        var tableData = MapToTableData(reportType, data);

        // 1. EXCEL EXPORT (Tabular Spreadsheet via ClosedXML)
        if (!pdf)
        {
            using var workbook = new XLWorkbook();

            var sheetName = string.IsNullOrWhiteSpace(reportType)
                ? "Report"
                : reportType.Trim();

            if (sheetName.Length > 31)
                sheetName = sheetName.Substring(0, 31);

            var worksheet = workbook.Worksheets.Add(sheetName);

            // Title Block
            worksheet.Cell(1, 1).Value = tableData.Title.ToUpperInvariant();
            worksheet.Cell(1, 1).Style.Font.Bold = true;
            worksheet.Cell(1, 1).Style.Font.FontSize = 14;
            worksheet.Cell(1, 1).Style.Font.FontColor = XLColor.FromHtml("#1E3A8A");

            worksheet.Cell(2, 1).Value = $"Generated on: {DateTime.Now:dd-MM-yyyy HH:mm:ss} | College Management System";
            worksheet.Cell(2, 1).Style.Font.Italic = true;
            worksheet.Cell(2, 1).Style.Font.FontColor = XLColor.FromHtml("#64748B");

            int currentRow = 4;

            // KPI Cards Block if present
            if (tableData.Kpis.Count > 0)
            {
                int kpiCol = 1;
                foreach (var kpi in tableData.Kpis)
                {
                    worksheet.Cell(currentRow, kpiCol).Value = kpi.Key;
                    worksheet.Cell(currentRow, kpiCol).Style.Font.Bold = true;
                    worksheet.Cell(currentRow, kpiCol).Style.Font.FontSize = 9;
                    worksheet.Cell(currentRow, kpiCol).Style.Fill.BackgroundColor = XLColor.FromHtml("#F1F5F9");

                    worksheet.Cell(currentRow + 1, kpiCol).Value = kpi.Value;
                    worksheet.Cell(currentRow + 1, kpiCol).Style.Font.Bold = true;
                    worksheet.Cell(currentRow + 1, kpiCol).Style.Font.FontSize = 11;
                    worksheet.Cell(currentRow + 1, kpiCol).Style.Font.FontColor = XLColor.FromHtml("#0F172A");
                    kpiCol++;
                }
                currentRow += 3;
            }

            // Table Headers
            int headerCol = 1;
            foreach (var h in tableData.Headers)
            {
                var cell = worksheet.Cell(currentRow, headerCol);
                cell.Value = h;
                cell.Style.Font.Bold = true;
                cell.Style.Font.FontColor = XLColor.White;
                cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#1E40AF");
                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                headerCol++;
            }
            currentRow++;

            // Data Rows
            bool alternate = false;
            foreach (var r in tableData.Rows)
            {
                for (int col = 0; col < r.Length; col++)
                {
                    var cell = worksheet.Cell(currentRow, col + 1);
                    cell.Value = r[col];
                    if (alternate)
                    {
                        cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#F8FAFC");
                    }
                    cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                    cell.Style.Border.OutsideBorderColor = XLColor.FromHtml("#E2E8F0");
                }
                alternate = !alternate;
                currentRow++;
            }

            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);

            return (
                stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"{reportType}-report.xlsx"
            );
        }

        // 2. PDF EXPORT (Tabular Document via QuestPDF)
        QuestPDF.Settings.License = LicenseType.Community;

        var pdfBytes = Document.Create(document =>
        {
            document.Page(page =>
            {
                page.Size(PageSizes.A4.Landscape());
                page.Margin(25);

                page.Header().Column(headerCol =>
                {
                    headerCol.Item().Row(row =>
                    {
                        row.RelativeItem().Column(titleCol =>
                        {
                            titleCol.Item().Text("COLLEGE MANAGEMENT SYSTEM").FontSize(9).Bold().FontColor("#64748B");
                            titleCol.Item().Text(tableData.Title).FontSize(16).Bold().FontColor("#1E3A8A");
                        });
                        row.ConstantItem(180).AlignRight().Text($"Date: {DateTime.Now:dd MMM yyyy, HH:mm}").FontSize(9).FontColor("#64748B");
                    });
                    headerCol.Item().PaddingTop(6).LineHorizontal(1.2f).LineColor("#CBD5E1");
                });

                page.Content().PaddingTop(12).Column(column =>
                {
                    if (tableData.Kpis.Count > 0)
                    {
                        column.Item().PaddingBottom(10).Row(kpiRow =>
                        {
                            foreach (var k in tableData.Kpis)
                            {
                                kpiRow.RelativeItem().Border(1).BorderColor("#E2E8F0").Background("#F8FAFC").Padding(6).Column(c =>
                                {
                                    c.Item().Text(k.Key).FontSize(7).FontColor("#64748B").Bold();
                                    c.Item().Text(k.Value).FontSize(11).FontColor("#0F172A").Bold();
                                });
                            }
                        });
                    }

                    column.Item().Table(table =>
                    {
                        table.ColumnsDefinition(cols =>
                        {
                            cols.ConstantColumn(30); // S.No / Rank
                            for (int i = 1; i < tableData.Headers.Length; i++)
                            {
                                cols.RelativeColumn();
                            }
                        });

                        table.Header(header =>
                        {
                            foreach (var h in tableData.Headers)
                            {
                                header.Cell().Background("#1E40AF").Padding(5).AlignCenter().Text(h).FontSize(8).Bold().FontColor("#FFFFFF");
                            }
                        });

                        int rowIdx = 0;
                        foreach (var r in tableData.Rows)
                        {
                            var bg = rowIdx % 2 == 1 ? "#F8FAFC" : "#FFFFFF";
                            for (int col = 0; col < r.Length; col++)
                            {
                                var cell = table.Cell().Background(bg).BorderBottom(1).BorderColor("#E2E8F0").Padding(4);
                                if (col == 0)
                                    cell.AlignCenter().Text(r[col]).FontSize(8);
                                else
                                    cell.AlignLeft().Text(r[col]).FontSize(8);
                            }
                            rowIdx++;
                        }
                    });
                });

                page.Footer().AlignCenter().Text(text =>
                {
                    text.Span("Page ");
                    text.CurrentPageNumber();
                    text.Span(" of ");
                    text.TotalPages();
                    text.Span(" | College Management System");
                });
            });
        }).GeneratePdf();

        return (
            pdfBytes,
            "application/pdf",
            $"{reportType}-report.pdf"
        );
    }

    private static (string Title, string[] Headers, List<string[]> Rows, List<(string Key, string Value)> Kpis) MapToTableData(string reportType, object data)
    {
        var rows = new List<string[]>();
        var kpis = new List<(string Key, string Value)>();
        string title = "Report";
        string[] headers = Array.Empty<string>();

        switch (data)
        {
            case DashboardReportDto d:
                title = "Dashboard Overview & KPI Summary";
                headers = new[] { "S.No", "Metric Name", "Metric Value", "Category" };
                kpis.Add(("Total Admissions", $"{d.Admissions}"));
                kpis.Add(("Attendance Rate", $"{d.Attendance:F2}%"));
                kpis.Add(("Fee Collections", $"₹{d.FeeCollection:N2}"));
                kpis.Add(("Outstanding Dues", $"₹{d.DueFees:N2}"));
                kpis.Add(("Student Strength", $"{d.StudentStrength}"));
                kpis.Add(("Pass Percentage", $"{d.PassPercentage:F1}%"));
                rows.Add(new[] { "1", "Total Admissions", $"{d.Admissions}", "Academics" });
                rows.Add(new[] { "2", "Average Student Attendance", $"{d.Attendance:F2}%", "Attendance" });
                rows.Add(new[] { "3", "Total Fee Collection", $"₹{d.FeeCollection:N2}", "Finance" });
                rows.Add(new[] { "4", "Total Outstanding Due Fees", $"₹{d.DueFees:N2}", "Finance" });
                rows.Add(new[] { "5", "Total Examinations Conducted", $"{d.Examinations}", "Examinations" });
                rows.Add(new[] { "6", "Published Exam Results", $"{d.ResultsPublished}", "Examinations" });
                rows.Add(new[] { "7", "Total Faculty Workload", $"{d.FacultyWorkload:F1} hrs/wk", "Staff" });
                rows.Add(new[] { "8", "Total Student Strength", $"{d.StudentStrength}", "Students" });
                rows.Add(new[] { "9", "Overall Pass Percentage", $"{d.PassPercentage:F1}%", "Academics" });
                rows.Add(new[] { "10", "Toppers Identified", $"{d.ToppersIdentified}", "Academics" });
                break;

            case IReadOnlyList<AdmissionReportDto> adm:
                title = "Student Admissions Report";
                headers = new[] { "S.No", "Period / Academic Session", "Total Admissions", "Approved", "Pending", "Rejected" };
                int aIdx = 1;
                foreach (var a in adm)
                    rows.Add(new[] { $"{aIdx++}", a.Period ?? "—", $"{a.Admissions}", $"{a.Approved}", $"{a.Pending}", $"{a.Rejected}" });
                break;

            case StudentStrengthReportDto ss:
                title = "Student Strength Breakdown Report";
                headers = new[] { "S.No", "Student ID", "Student Name", "Gender", "Group", "Section" };
                kpis.Add(("Total Students", $"{ss.TotalStudents}"));
                kpis.Add(("Male Students", $"{ss.MaleStudents}"));
                kpis.Add(("Female Students", $"{ss.FemaleStudents}"));
                kpis.Add(("Other Students", $"{ss.OtherStudents}"));
                int ssIdx = 1;
                foreach (var s in ss.Students)
                    rows.Add(new[] { $"{ssIdx++}", $"{s.StudentId}", s.StudentName ?? "—", s.Gender ?? "—", s.GroupName ?? "—", s.SectionName ?? "—" });
                break;

            case IReadOnlyList<AttendanceReportDto> att:
                title = "Student Attendance Report";
                headers = new[] { "S.No", "Period / Date", "Present", "Absent", "Late", "Leave", "Attendance %" };
                int attIdx = 1;
                foreach (var a in att)
                    rows.Add(new[] { $"{attIdx++}", a.Period ?? "—", $"{a.Present}", $"{a.Absent}", $"{a.Late}", $"{a.Leave}", $"{a.AttendancePercentage:F1}%" });
                break;

            case IReadOnlyList<FacultyAttendanceReportDto> fa:
                title = "Faculty & Staff Attendance Report";
                headers = new[] { "S.No", "Faculty ID", "Faculty Name", "Present", "Absent", "Late", "Leave", "Attendance %" };
                int faIdx = 1;
                foreach (var f in fa)
                    rows.Add(new[] { $"{faIdx++}", $"{f.FacultyId}", f.FacultyName ?? "—", $"{f.Present}", $"{f.Absent}", $"{f.Late}", $"{f.Leave}", $"{f.AttendancePercentage:F1}%" });
                break;

            case IReadOnlyList<FeeCollectionReportDto> fees:
                title = "Fee Collection & Transactions Report";
                headers = new[] { "S.No", "Period / Date", "Collected Amount", "Discounts", "Fines", "Transactions" };
                int feeIdx = 1;
                foreach (var f in fees)
                    rows.Add(new[] { $"{feeIdx++}", f.Period ?? "—", $"₹{f.Collected:N2}", $"₹{f.Discount:N2}", $"₹{f.Fine:N2}", $"{f.Transactions}" });
                break;

            case IReadOnlyList<OutstandingFeeReportDto> dues:
                title = "Outstanding Due Fees & Defaulters Report";
                headers = new[] { "S.No", "Admission No", "Roll No", "Student Name", "Total Fee", "Paid Fee", "Due Amount", "Fee Status" };
                int dueIdx = 1;
                foreach (var d in dues)
                    rows.Add(new[] { $"{dueIdx++}", d.AdmissionNo ?? "—", d.RollNo ?? "—", d.StudentName ?? "—", $"₹{d.TotalAmount:N2}", $"₹{d.PaidAmount:N2}", $"₹{d.DueAmount:N2}", d.FeeStatus ?? "—" });
                break;

            case IReadOnlyList<ExaminationReportDto> exams:
                title = "Examinations Master & Schedules Report";
                headers = new[] { "S.No", "Exam Code", "Exam Name", "Academic Year", "Group", "Type", "Start Date", "End Date", "Status", "Eligible", "Pass %" };
                int exIdx = 1;
                foreach (var e in exams)
                    rows.Add(new[] { $"{exIdx++}", e.ExamCode ?? "—", e.ExamName ?? "—", e.AcademicYear ?? "—", e.GroupName ?? "—", e.ExamType ?? "—", e.StartDate ?? "—", e.EndDate ?? "—", e.Status ?? "—", $"{e.TotalEligibleStudents}", $"{e.PassPercentage:F1}%" });
                break;

            case IReadOnlyList<ResultAnalysisReportDto> results:
                title = "Examination Results Analysis Report";
                headers = new[] { "S.No", "Exam Name", "Total Results", "Passed", "Failed", "Average Score %" };
                int resIdx = 1;
                foreach (var r in results)
                    rows.Add(new[] { $"{resIdx++}", r.ExamName ?? "—", $"{r.TotalResults}", $"{r.Passed}", $"{r.Failed}", $"{r.AveragePercentage:F1}%" });
                break;

            case IReadOnlyList<PassPercentageReportDto> pp:
                title = "Academic Pass Percentage Report";
                headers = new[] { "S.No", "Exam Name", "Passed", "Failed", "Pass Percentage %" };
                int ppIdx = 1;
                foreach (var p in pp)
                    rows.Add(new[] { $"{ppIdx++}", p.ExamName ?? "—", $"{p.Passed}", $"{p.Failed}", $"{p.PassPercentage:F1}%" });
                break;

            case IReadOnlyList<TopperReportDto> tops:
                title = "Student Toppers & Rankers Leaderboard";
                headers = new[] { "Rank", "Student Name", "Roll No", "Group", "Section", "Total Marks", "Percentage %", "Passed Subjects" };
                int tIdx = 1;
                foreach (var t in tops)
                    rows.Add(new[] { t.Rank > 0 ? $"{t.Rank}" : $"{tIdx++}", t.StudentName ?? "—", t.RollNo ?? "—", t.GroupName ?? "—", t.SectionName ?? "—", $"{t.TotalMarks:F0}", $"{t.Percentage:F1}%", $"{t.PassedSubjects}" });
                break;

            case IReadOnlyList<FacultyWorkloadReportDto> fw:
                title = "Faculty Workload & Allocation Report";
                headers = new[] { "S.No", "Faculty ID", "Faculty Name", "Assigned Periods", "Weekly Hours" };
                int fwIdx = 1;
                foreach (var f in fw)
                    rows.Add(new[] { $"{fwIdx++}", $"{f.FacultyId}", f.FacultyName ?? "—", $"{f.PeriodCount}", $"{f.HoursPerWeek:F1} hrs" });
                break;

            case IReadOnlyList<StudentPerformanceReportDto> sp:
                title = "Student Academic Performance Report";
                headers = new[] { "S.No", "Admission No", "Roll No", "Student Name", "Average %", "Passed", "Failed", "Attendance %", "Grade" };
                int spIdx = 1;
                foreach (var s in sp)
                    rows.Add(new[] { $"{spIdx++}", s.AdmissionNo ?? "—", s.RollNo ?? "—", s.StudentName ?? "—", $"{s.AveragePercentage:F1}%", $"{s.PassedSubjects}", $"{s.FailedSubjects}", $"{s.AttendancePercentage:F1}%", s.Grade ?? "—" });
                break;

            case IReadOnlyList<AuditLogDto> logs:
                title = "System Security & User Activity Audit Logs";
                headers = new[] { "S.No", "User", "Action", "Entity", "Description", "Timestamp" };
                int logIdx = 1;
                foreach (var l in logs)
                    rows.Add(new[] { $"{logIdx++}", l.UserName ?? "System", l.Action ?? "—", l.EntityName ?? "—", l.Description ?? "—", l.CreatedAt.ToString("dd-MM-yyyy HH:mm") });
                break;

            default:
                title = $"{char.ToUpper(reportType[0])}{reportType.Substring(1)} Report";
                headers = new[] { "S.No", "Information", "Value" };
                rows.Add(new[] { "1", "Report Status", "Completed" });
                break;
        }

        if (headers.Length == 0)
        {
            headers = new[] { "S.No", "Item", "Status" };
            rows.Add(new[] { "1", "No records found for the selected filter criteria.", "—" });
        }
        else if (rows.Count == 0)
        {
            var emptyRow = new string[headers.Length];
            emptyRow[0] = "1";
            emptyRow[1] = "No records found for the selected filters.";
            for (int i = 2; i < headers.Length; i++) emptyRow[i] = "—";
            rows.Add(emptyRow);
        }

        return (title, headers, rows, kpis);
    }
}