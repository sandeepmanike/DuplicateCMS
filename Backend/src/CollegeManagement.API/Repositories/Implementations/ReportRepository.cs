using System.Data;
using CollegeManagement.API.Data;
using CollegeManagement.API.DTOs.Reports;
using CollegeManagement.API.Models.Reports;
using CollegeManagement.API.Repositories.Interfaces;
using Dapper;
using Microsoft.EntityFrameworkCore;

namespace CollegeManagement.API.Repositories.Implementations;

public class ReportRepository : IReportRepository
{
    private readonly AppDbContext _context;
    public ReportRepository(AppDbContext context) => _context = context;
    private IDbConnection Connection => _context.Database.GetDbConnection();

    private static object P(ReportFilterModel f) => new
    {
        p_BoardId = f.BoardId,
        p_AcademicYearId = f.AcademicYearId,
        p_AcademicLevelId = f.AcademicLevelId,
        p_GroupId = f.GroupId,
        p_SectionId = f.SectionId,
        p_FromDate = f.FromDate,
        p_ToDate = f.ToDate
    };

    private async Task<IReadOnlyList<T>> QueryAsync<T>(string procedure, ReportFilterModel filter, CancellationToken ct)
    {
        var command = new CommandDefinition(procedure, P(filter), commandType: CommandType.StoredProcedure, cancellationToken: ct);
        var rows = await Connection.QueryAsync<T>(command);
        return rows.AsList();
    }

    public async Task<DashboardReportDto> GetDashboardAsync(ReportFilterModel filter, CancellationToken ct = default)
    {
        var command = new CommandDefinition("sp_Report_Dashboard", P(filter), commandType: CommandType.StoredProcedure, cancellationToken: ct);
        using var multi = await Connection.QueryMultipleAsync(command);
        var summary = await multi.ReadFirstOrDefaultAsync<DashboardReportDto>() ?? new DashboardReportDto();
        summary.AdmissionsVsTarget = (await multi.ReadAsync<TrendPointDto>()).AsList();
        summary.AttendanceTrend = (await multi.ReadAsync<TrendPointDto>()).AsList();
        summary.FeeCollectedVsDue = (await multi.ReadAsync<TrendPointDto>()).AsList();
        summary.Toppers = (await multi.ReadAsync<TopperReportDto>()).AsList();
        return summary;
    }

    public Task<IReadOnlyList<AdmissionReportDto>> GetAdmissionsAsync(ReportFilterModel f, CancellationToken ct = default) => QueryAsync<AdmissionReportDto>("sp_Report_Admissions", f, ct);
    public Task<IReadOnlyList<StudentStrengthReportDto>> GetStudentStrengthAsync(ReportFilterModel f, CancellationToken ct = default) => QueryAsync<StudentStrengthReportDto>("sp_Report_StudentStrength", f, ct);
    public Task<IReadOnlyList<AttendanceReportDto>> GetAttendanceAsync(ReportFilterModel f, CancellationToken ct = default) => QueryAsync<AttendanceReportDto>("sp_Report_Attendance", f, ct);
    public Task<IReadOnlyList<FacultyAttendanceReportDto>> GetFacultyAttendanceAsync(ReportFilterModel f, CancellationToken ct = default) => QueryAsync<FacultyAttendanceReportDto>("sp_Report_FacultyAttendance", f, ct);
    public Task<IReadOnlyList<FeeCollectionReportDto>> GetFeeCollectionAsync(ReportFilterModel f, CancellationToken ct = default) => QueryAsync<FeeCollectionReportDto>("sp_Report_FeeCollection", f, ct);
    public Task<IReadOnlyList<OutstandingFeeReportDto>> GetOutstandingFeesAsync(ReportFilterModel f, CancellationToken ct = default) => QueryAsync<OutstandingFeeReportDto>("sp_Report_OutstandingFees", f, ct);
    public Task<IReadOnlyList<ExaminationReportDto>> GetExaminationsAsync(ReportFilterModel f, CancellationToken ct = default) => QueryAsync<ExaminationReportDto>("sp_Report_Examinations", f, ct);
    public Task<IReadOnlyList<ResultAnalysisReportDto>> GetResultsAsync(ReportFilterModel f, CancellationToken ct = default) => QueryAsync<ResultAnalysisReportDto>("sp_Report_Results", f, ct);
    public Task<IReadOnlyList<PassPercentageReportDto>> GetPassPercentageAsync(ReportFilterModel f, CancellationToken ct = default) => QueryAsync<PassPercentageReportDto>("sp_Report_PassPercentage", f, ct);
    public Task<IReadOnlyList<TopperReportDto>> GetToppersAsync(ReportFilterModel f, CancellationToken ct = default) => QueryAsync<TopperReportDto>("sp_Report_Toppers", f, ct);
    public Task<IReadOnlyList<SubjectWiseReportDto>> GetSubjectsAsync(ReportFilterModel f, CancellationToken ct = default) => QueryAsync<SubjectWiseReportDto>("sp_Report_Subjects", f, ct);
    public Task<IReadOnlyList<GroupWiseReportDto>> GetGroupsAsync(ReportFilterModel f, CancellationToken ct = default) => QueryAsync<GroupWiseReportDto>("sp_Report_Groups", f, ct);
    public Task<IReadOnlyList<SectionWiseReportDto>> GetSectionsAsync(ReportFilterModel f, CancellationToken ct = default) => QueryAsync<SectionWiseReportDto>("sp_Report_Sections", f, ct);
    public Task<IReadOnlyList<FacultyWorkloadReportDto>> GetFacultyWorkloadAsync(ReportFilterModel f, CancellationToken ct = default) => QueryAsync<FacultyWorkloadReportDto>("sp_Report_FacultyWorkload", f, ct);
    public Task<IReadOnlyList<StudentPerformanceReportDto>> GetStudentPerformanceAsync(ReportFilterModel f, CancellationToken ct = default) => QueryAsync<StudentPerformanceReportDto>("sp_Report_StudentPerformance", f, ct);
    public Task<IReadOnlyList<AuditLogDto>> GetAuditLogsAsync(ReportFilterModel f, CancellationToken ct = default) => QueryAsync<AuditLogDto>("sp_Report_AuditLogs", f, ct);
}
