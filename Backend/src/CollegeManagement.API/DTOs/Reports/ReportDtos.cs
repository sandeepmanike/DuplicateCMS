using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CollegeManagement.API.DTOs.Reports;

public class ReportFilterDto
{
    public int? BoardId { get; set; }
    public int? AcademicYearId { get; set; }
    public int? AcademicLevelId { get; set; }
    public int? GroupId { get; set; }
    public int? SectionId { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}

public class DashboardReportDto
{
    public int Admissions { get; set; }
    public decimal Attendance { get; set; }
    public decimal FeeCollection { get; set; }
    public decimal DueFees { get; set; }
    public int Examinations { get; set; }
    public int ResultsPublished { get; set; }
    public decimal FacultyWorkload { get; set; }
    public int StudentStrength { get; set; }
    public decimal PassPercentage { get; set; }
    public int ToppersIdentified { get; set; }
    public IReadOnlyList<TrendPointDto> AdmissionsVsTarget { get; set; } = [];
    public IReadOnlyList<TrendPointDto> AttendanceTrend { get; set; } = [];
    public IReadOnlyList<TrendPointDto> FeeCollectedVsDue { get; set; } = [];
    public IReadOnlyList<TopperReportDto> Toppers { get; set; } = [];
}

public class TrendPointDto
{
    public string? Label { get; set; } = string.Empty;
    public decimal Value { get; set; }
    public decimal Target { get; set; }
    public decimal Due { get; set; }
}

public class AdmissionReportDto
{
    public string? Period { get; set; } = string.Empty;
    public int Admissions { get; set; }
    public int Approved { get; set; }
    public int Rejected { get; set; }
    public int Pending { get; set; }
}

public class StudentStrengthReportDto
{
    public int TotalStudents { get; set; }
    public int MaleStudents { get; set; }
    public int FemaleStudents { get; set; }
    public int OtherStudents { get; set; }

    public IReadOnlyList<StudentStrengthStudentDto> Students { get; set; } = [];
}

public class StudentStrengthStudentDto
{
    public int StudentId { get; set; }
    public string? StudentName { get; set; } = string.Empty;
    public string? Gender { get; set; } = string.Empty;
    public string? GroupName { get; set; } = string.Empty;
    public string? SectionName { get; set; } = string.Empty;
}

public class AttendanceReportDto
{
    public string? Period { get; set; } = string.Empty;
    public int Present { get; set; }
    public int Absent { get; set; }
    public int Late { get; set; }
    public int Leave { get; set; }
    public decimal AttendancePercentage { get; set; }
}

public class FacultyAttendanceReportDto
{
    public int FacultyId { get; set; }
    public string? FacultyName { get; set; } = string.Empty;
    public int Present { get; set; }
    public int Absent { get; set; }
    public int Late { get; set; }
    public int Leave { get; set; }
    public decimal AttendancePercentage { get; set; }
}

public class FeeCollectionReportDto
{
    public string? Period { get; set; } = string.Empty;
    public decimal Collected { get; set; }
    public decimal Discount { get; set; }
    public decimal Fine { get; set; }
    public int Transactions { get; set; }
}

public class OutstandingFeeReportDto
{
    public int StudentId { get; set; }
    public string? AdmissionNo { get; set; } = string.Empty;
    public string? RollNo { get; set; } = string.Empty;
    public string? StudentName { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal DueAmount { get; set; }
    public string? FeeStatus { get; set; } = string.Empty;
}

public class ExaminationReportDto
{
    public int ExaminationId { get; set; }
    public string? ExamCode { get; set; } = string.Empty;
    public string? ExamName { get; set; } = string.Empty;
    public string? BoardName { get; set; } = string.Empty;
    public string? AcademicYear { get; set; } = string.Empty;
    public string? AcademicLevel { get; set; } = string.Empty;
    public string? GroupName { get; set; } = string.Empty;
    public string? ProgramName { get; set; } = string.Empty;
    public string? ExamType { get; set; } = string.Empty;
    public string? StartDate { get; set; } = string.Empty;
    public string? EndDate { get; set; } = string.Empty;
    public string? Status { get; set; } = string.Empty;
    public int TotalEligibleSubjects { get; set; }
    public int ScheduledSubjectsCount { get; set; }
    public int TotalEligibleStudents { get; set; }
    public int HallTicketsGeneratedCount { get; set; }
    public int ResultCount { get; set; }
    public int PublishedCount { get; set; }
    public decimal PassPercentage { get; set; }
}

public class ResultAnalysisReportDto
{
    public string? ExamName { get; set; } = string.Empty;
    public int TotalResults { get; set; }
    public int Passed { get; set; }
    public int Failed { get; set; }
    public decimal AveragePercentage { get; set; }
}

public class PassPercentageReportDto
{
    public string? ExamName { get; set; } = string.Empty;
    public decimal PassPercentage { get; set; }
    public int Passed { get; set; }
    public int Failed { get; set; }
}

public class TopperReportDto
{
    public int Rank { get; set; }
    public int StudentId { get; set; }
    public string? StudentName { get; set; } = string.Empty;
    public string? RollNo { get; set; } = string.Empty;
    public int GroupId { get; set; }
    public string? GroupName { get; set; } = string.Empty;
    public int SectionId { get; set; }
    public string? SectionName { get; set; } = string.Empty;
    public int DepartmentId { get; set; }
    public string? DepartmentName { get; set; } = string.Empty;
    public int ProgramId { get; set; }
    public string? ProgramName { get; set; } = string.Empty;
    public int Subjects { get; set; }
    public decimal TotalMarks { get; set; }
    public decimal Percentage { get; set; }
    public int PassedSubjects { get; set; }
    public int FailedSubjects { get; set; }
}

public class SubjectWiseReportDto
{
    public int SubjectId { get; set; }
    public string? SubjectName { get; set; } = string.Empty;
    public int Students { get; set; }
    public decimal AverageMarks { get; set; }
    public decimal PassPercentage { get; set; }
}

public class GroupWiseReportDto
{
    public int GroupId { get; set; }
    public string? GroupName { get; set; } = string.Empty;
    public int StudentCount { get; set; }
    public decimal AveragePercentage { get; set; }
    public decimal PassPercentage { get; set; }
}

public class SectionWiseReportDto
{
    public int SectionId { get; set; }
    public string? SectionName { get; set; } = string.Empty;
    public string? GroupName { get; set; } = string.Empty;
    public int StudentCount { get; set; }
    public decimal AveragePercentage { get; set; }
    public decimal PassPercentage { get; set; }
}

public class FacultyWorkloadReportDto
{
    public int FacultyId { get; set; }
    public string? FacultyName { get; set; } = string.Empty;
    public int PeriodCount { get; set; }
    public decimal HoursPerWeek { get; set; }
}

public class StudentPerformanceReportDto
{
    public int StudentId { get; set; }
    public string? AdmissionNo { get; set; } = string.Empty;
    public string? RollNo { get; set; } = string.Empty;
    public string? StudentName { get; set; } = string.Empty;
    public decimal AveragePercentage { get; set; }
    public int PassedSubjects { get; set; }
    public int FailedSubjects { get; set; }
    public decimal AttendancePercentage { get; set; }
    public string? Grade { get; set; }
}

public class AuditLogDto
{
    public long AuditLogId { get; set; }
    public string? UserName { get; set; }
    public string? Action { get; set; } = string.Empty;
    public string? EntityName { get; set; } = string.Empty;
    public int? EntityId { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CustomReportRequestDto : ReportFilterDto
{
    [Required, MaxLength(60)]
    public string ReportType { get; set; } = string.Empty;
}
