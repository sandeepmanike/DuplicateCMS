namespace CollegeManagement.API.DTOs.Students
{
    public class StudentListItemDto
    {
        public int StudentId { get; set; }
        public string? AdmissionNo { get; set; }
        public string? RollNo { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string? Photo { get; set; }
        public string? Gender { get; set; }
        public string? Email { get; set; }
        public string? MobileNumber { get; set; }

        public int? BoardId { get; set; }
        public string? BoardName { get; set; }

        public int? AcademicYearId { get; set; }
        public string? AcademicYearName { get; set; }

        public int? AcademicLevelId { get; set; }
        public string? AcademicLevelName { get; set; }

        public int? GroupId { get; set; }
        public string? GroupName { get; set; }

        public int? SectionId { get; set; }
        public string? SectionName { get; set; }

        public bool IsActive { get; set; }
        public string Status { get; set; } = "Active";
        public DateTime CreatedAt { get; set; }
    }
}
