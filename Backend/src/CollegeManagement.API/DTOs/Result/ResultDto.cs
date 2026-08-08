using System;

namespace CollegeManagement.API.DTOs.Result
{
    public class ResultDto
    {
        public int ResultId { get; set; }

        public string RollNumber { get; set; } = string.Empty;
        public int StudentId { get; set; }

        public string StudentName { get; set; } = string.Empty;

        public int BoardId { get; set; }

        public string BoardName { get; set; } = string.Empty;

        public int AcademicYearId { get; set; }

        public string AcademicYear { get; set; } = string.Empty;

        public int AcademicLevelId { get; set; }

        public string AcademicLevel { get; set; } = string.Empty;

        public int GroupId { get; set; }

        public string GroupName { get; set; } = string.Empty;

        public int ExamId { get; set; }

        public string ExamName { get; set; } = string.Empty;

        public int SubjectId { get; set; }

        public string SubjectName { get; set; } = string.Empty;

        public decimal InternalMarks { get; set; }

        public decimal PracticalMarks { get; set; }

        public decimal ExternalMarks { get; set; }

        public decimal TotalMarks { get; set; }

        public string Grade { get; set; } = string.Empty;

        public string ResultStatus { get; set; } = string.Empty;

        public int? Rank { get; set; }

        public bool IsPublished { get; set; }

        public DateTime? PublishedDate { get; set; }
    }
}