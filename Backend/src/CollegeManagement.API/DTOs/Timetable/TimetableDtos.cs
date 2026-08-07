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
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
