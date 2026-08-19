using System;

namespace CollegeManagement.API.DTOs.Sections
{
    public class SectionResponse
    {
        public int SectionId { get; set; }
        public string Board { get; set; } = string.Empty;
        public int AcademicYearId { get; set; }
        public string AcademicYearName { get; set; } = string.Empty;
        public string Group { get; set; } = string.Empty;
        public string AcademicLevel { get; set; } = string.Empty;
        public string SectionName { get; set; } = string.Empty;
        public string? RoomNumber { get; set; }
        public int? RoomId { get; set; }
        public string? RoomName { get; set; }
        public int? ClassTeacherId { get; set; }
        public string ClassTeacherName { get; set; } = string.Empty;
        public int MaximumStrength { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
