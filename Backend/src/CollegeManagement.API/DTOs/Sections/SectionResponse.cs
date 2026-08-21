using System;

namespace CollegeManagement.API.DTOs.Sections
{
    public class SectionResponse
    {
        public int SectionId { get; set; }
        public int? BoardId { get; set; }
        public string Board { get; set; } = string.Empty;
        public int AcademicYearId { get; set; }
        public string AcademicYearName { get; set; } = string.Empty;
        public int? GroupId { get; set; }
        public string Group { get; set; } = string.Empty;
        public string Programme { get; set; } = string.Empty;
        public string Program => Programme;
        public string AcademicLevel { get; set; } = string.Empty;
        public string YearOfStudy => AcademicLevel;
        public string SectionName { get; set; } = string.Empty;
        public string? RoomNumber { get; set; }
        public int? RoomId { get; set; }
        public string? RoomName { get; set; }
        public int? ClassTeacherId { get; set; }
        public string ClassTeacherName { get; set; } = string.Empty;
        public int MaximumStrength { get; set; }
        public int Capacity => MaximumStrength;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
