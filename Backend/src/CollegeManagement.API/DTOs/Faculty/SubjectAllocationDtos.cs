using System;

namespace CollegeManagement.API.DTOs.Faculty.Request
{
    public class AssignSubjectDto
    {
        public int FacultyId { get; set; }
        public int BoardId { get; set; }
        public string Board { get; set; } = string.Empty;
        public int AcademicLevelId { get; set; }
        public string AcademicLevel { get; set; } = string.Empty;
        public int AcademicYearId { get; set; }
        public string AcademicYear { get; set; } = string.Empty;
        public int GroupId { get; set; }
        public string Group { get; set; } = string.Empty;
        public int SectionId { get; set; }
        public string Section { get; set; } = string.Empty;
        public int SubjectId { get; set; }
        public string Subject { get; set; } = string.Empty;
    }

    public class UpdateSubjectAllocationDto
    {
        public int BoardId { get; set; }
        public string Board { get; set; } = string.Empty;
        public int AcademicLevelId { get; set; }
        public string AcademicLevel { get; set; } = string.Empty;
        public int AcademicYearId { get; set; }
        public string AcademicYear { get; set; } = string.Empty;
        public int GroupId { get; set; }
        public string Group { get; set; } = string.Empty;
        public int SectionId { get; set; }
        public string Section { get; set; } = string.Empty;
        public int SubjectId { get; set; }
        public string Subject { get; set; } = string.Empty;
    }

    public class FacultySubjectAllocationResponseDto
    {
        public int Id { get; set; }
        public int FacultyId { get; set; }
        public string FacultyName { get; set; } = string.Empty;
        public int BoardId { get; set; }
        public string BoardName { get; set; } = string.Empty;
        public int AcademicLevelId { get; set; }
        public string AcademicLevelName { get; set; } = string.Empty;
        public int AcademicYearId { get; set; }
        public string AcademicYearName { get; set; } = string.Empty;
        public int GroupId { get; set; }
        public string GroupName { get; set; } = string.Empty;
        public int SectionId { get; set; }
        public string SectionName { get; set; } = string.Empty;
        public int SubjectId { get; set; }
        public string SubjectName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
