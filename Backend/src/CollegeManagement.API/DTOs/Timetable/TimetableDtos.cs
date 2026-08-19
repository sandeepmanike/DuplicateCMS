using System;
using System.Collections.Generic;

namespace CollegeManagement.API.DTOs.Timetable
{
    public class CreateTimetableDto
    {
        public int BoardId { get; set; }
        public int AcademicLevelId { get; set; }
        public int AcademicYearId { get; set; }
        public int GroupId { get; set; }
        public int SectionId { get; set; }
        public int DayOfWeek { get; set; } // 1=Monday .. 6=Saturday
        public int PeriodId { get; set; }
        public int SubjectId { get; set; }
        public int FacultyId { get; set; }
        public int RoomId { get; set; }
        public bool IsPublished { get; set; } = false;
        public string? Remarks { get; set; }
    }

    public class UpdateTimetableDto
    {
        public int BoardId { get; set; }
        public int AcademicLevelId { get; set; }
        public int AcademicYearId { get; set; }
        public int GroupId { get; set; }
        public int SectionId { get; set; }
        public int DayOfWeek { get; set; }
        public int PeriodId { get; set; }
        public int SubjectId { get; set; }
        public int FacultyId { get; set; }
        public int RoomId { get; set; }
        public bool IsPublished { get; set; } = false;
        public string? Remarks { get; set; }
    }

    public class CopyTimetableDto
    {
        public int SourceAcademicYearId { get; set; }
        public int SourceSectionId { get; set; }
        public int TargetAcademicYearId { get; set; }
        public int TargetSectionId { get; set; }
    }

    public class PublishTimetableDto
    {
        public bool IsPublished { get; set; }
    }

    public class AllocatedFacultyDto
    {
        public int FacultyId { get; set; }
        public string FacultyEmployeeId { get; set; } = string.Empty;
        public string FacultyName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Mobile { get; set; } = string.Empty;
        public string Designation { get; set; } = string.Empty;
    }

    public class TimetableResponseDto
    {
        public int Id { get; set; }

        public int BoardId { get; set; }
        public string BoardCode { get; set; } = string.Empty;
        public string BoardName { get; set; } = string.Empty;

        public int AcademicLevelId { get; set; }
        public string LevelCode { get; set; } = string.Empty;
        public string LevelName { get; set; } = string.Empty;

        public int AcademicYearId { get; set; }
        public string AcademicYearName { get; set; } = string.Empty;

        public int GroupId { get; set; }
        public string GroupCode { get; set; } = string.Empty;
        public string GroupName { get; set; } = string.Empty;

        public int SectionId { get; set; }
        public string SectionName { get; set; } = string.Empty;

        public int DayOfWeek { get; set; }
        public string DayName { get; set; } = string.Empty;

        public int PeriodId { get; set; }
        public string PeriodName { get; set; } = string.Empty;
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public bool IsBreak { get; set; }

        public int SubjectId { get; set; }
        public string SubjectCode { get; set; } = string.Empty;
        public string SubjectName { get; set; } = string.Empty;

        public int FacultyId { get; set; }
        public string FacultyEmployeeId { get; set; } = string.Empty;
        public string FacultyName { get; set; } = string.Empty;

        public int RoomId { get; set; }
        public string RoomCode { get; set; } = string.Empty;
        public string RoomName { get; set; } = string.Empty;

        public bool IsPublished { get; set; }
        public int ApprovalStatus { get; set; } = 0; // 0=Draft, 1=Approved, 2=Published
        public string ApprovalStatusName { get; set; } = "Draft";
        public string? Remarks { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class TimetableQueryParams
    {
        public int? BoardId { get; set; }
        public int? AcademicLevelId { get; set; }
        public int? AcademicYearId { get; set; }
        public int? GroupId { get; set; }
        public int? SectionId { get; set; }
        public int? DayOfWeek { get; set; }
        public int? FacultyId { get; set; }
        public int? RoomId { get; set; }
        public bool? IsPublished { get; set; }
        public int? ApprovalStatus { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }

    public class GenerateTimetableRequestDto
    {
        public int BoardId { get; set; }
        public int AcademicLevelId { get; set; }
        public int AcademicYearId { get; set; }
        public int GroupId { get; set; }

        public List<int> SectionIds { get; set; } = new List<int>();

        public List<int> WorkingDays { get; set; } = new List<int> { 1, 2, 3, 4, 5, 6 };

        public List<SubjectWeeklyPeriodRequirementDto> SubjectRequirements { get; set; }
            = new List<SubjectWeeklyPeriodRequirementDto>();
    }

    public class SubjectWeeklyPeriodRequirementDto
    {
        public int SubjectId { get; set; }
        public int WeeklyPeriods { get; set; }
    }

    public class GenerateTimetableResultDto
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public int TotalSlotsGenerated { get; set; }
        public int SectionsProcessedCount { get; set; }

        public List<TimetableResponseDto> GeneratedSlots { get; set; } = new List<TimetableResponseDto>();

        public List<UnassignedSlotWarningDto> Warnings { get; set; } = new List<UnassignedSlotWarningDto>();
    }

    public class UnassignedSlotWarningDto
    {
        public int SectionId { get; set; }
        public string SectionName { get; set; } = string.Empty;
        public int SubjectId { get; set; }
        public string SubjectName { get; set; } = string.Empty;
        public int UnassignedPeriodsCount { get; set; }
        public string Reason { get; set; } = string.Empty;
    }

    public class ValidateTimetableResultDto
    {
        public bool IsValid { get; set; }
        public int SectionId { get; set; }
        public string SectionName { get; set; } = string.Empty;
        public int AcademicYearId { get; set; }
        public int TotalSlots { get; set; }
        public List<TimetableValidationErrorDto> Errors { get; set; } = new List<TimetableValidationErrorDto>();
        public List<TimetableValidationErrorDto> Warnings { get; set; } = new List<TimetableValidationErrorDto>();
    }

    public class TimetableValidationErrorDto
    {
        public int? TimetableId { get; set; }
        public int DayOfWeek { get; set; }
        public string DayName { get; set; } = string.Empty;
        public int PeriodId { get; set; }
        public string PeriodName { get; set; } = string.Empty;
        public int SubjectId { get; set; }
        public string SubjectName { get; set; } = string.Empty;
        public int FacultyId { get; set; }
        public string FacultyName { get; set; } = string.Empty;
        public int RoomId { get; set; }
        public string RoomName { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
    }

    public class ApproveTimetableResultDto
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public int SectionId { get; set; }
        public int AcademicYearId { get; set; }
        public int TotalSlotsApproved { get; set; }
        public List<TimetableResponseDto> ApprovedSlots { get; set; } = new List<TimetableResponseDto>();
    }
}
