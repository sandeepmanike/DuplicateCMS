using System;

namespace CollegeManagement.API.DTOs.Faculty.Request
{
    public class AssignSubjectDto
    {
        public int FacultyId { get; set; }
        public int SubjectId { get; set; }

        // Optional fallback properties for frontend flexibility
        public string? Subject { get; set; }
        public string? SubjectName { get; set; }
        public string? SubjectCode { get; set; }
    }

    public class UpdateSubjectAllocationDto
    {
        public int SubjectId { get; set; }

        // Optional fallback properties for frontend flexibility
        public string? Subject { get; set; }
        public string? SubjectName { get; set; }
        public string? SubjectCode { get; set; }
    }

    public class FacultySubjectAllocationResponseDto
    {
        private int _id;
        public int Id { get => _id; set => _id = value; }
        public int AllocationId { get => _id; set => _id = value; }

        public int FacultyId { get; set; }
        public string FacultyName { get; set; } = string.Empty;

        public int SubjectId { get; set; }
        public string SubjectCode { get; set; } = string.Empty;

        private string _subjectName = string.Empty;
        public string Subject { get => _subjectName; set => _subjectName = value ?? string.Empty; }
        public string SubjectName { get => _subjectName; set => _subjectName = value ?? string.Empty; }

        private string _board = string.Empty;
        public string Board { get => _board; set => _board = value ?? string.Empty; }
        public string BoardName { get => _board; set => _board = value ?? string.Empty; }

        private string _group = string.Empty;
        public string Group { get => _group; set => _group = value ?? string.Empty; }
        public string GroupName { get => _group; set => _group = value ?? string.Empty; }

        private string _academicLevel = string.Empty;
        public string AcademicLevel { get => _academicLevel; set => _academicLevel = value ?? string.Empty; }
        public string AcademicLevelName { get => _academicLevel; set => _academicLevel = value ?? string.Empty; }

        public string Section { get; set; } = string.Empty;
        public string SectionName { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
    }
}
