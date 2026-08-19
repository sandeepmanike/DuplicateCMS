using System;
using System.Collections.Generic;

namespace CollegeManagement.API.DTOs.Evaluations
{
    public class StudentAnalysisDetailDto
    {
        public int StudentId { get; set; }
        public string RollNo { get; set; } = string.Empty;
        public string StudentName { get; set; } = string.Empty;
        public string GroupName { get; set; } = string.Empty;
        public string SectionName { get; set; } = string.Empty;
        public string ExamName { get; set; } = string.Empty;
        public decimal TotalMarks { get; set; }
        public decimal MaxMarks { get; set; }
        public decimal Percentage { get; set; }
        public string Grade { get; set; } = "F";
        public int? Rank { get; set; }
        public List<StudentSubjectAnalysisDetailItemDto> Subjects { get; set; } = new();
    }

    public class StudentSubjectAnalysisDetailItemDto
    {
        public int SubjectId { get; set; }
        public string SubjectName { get; set; } = string.Empty;
        public string SubjectCode { get; set; } = string.Empty;
        public decimal? Internal { get; set; }
        public decimal? Practical { get; set; }
        public decimal? Theory { get; set; }
        public decimal Total { get; set; }
        public int PassingMarks { get; set; } = 35;
        public bool IsAbsent { get; set; }
        public string? Remarks { get; set; }
    }
}
