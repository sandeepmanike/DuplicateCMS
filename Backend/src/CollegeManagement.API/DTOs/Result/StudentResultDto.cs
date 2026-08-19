namespace CollegeManagement.API.DTOs.Result
{
    public class StudentResultDto
    {
        public int StudentId { get; set; }

        public string StudentName { get; set; } = string.Empty;

        public string RollNumber { get; set; } = string.Empty;

        public string GroupName { get; set; } = string.Empty;

        public int ExamId { get; set; }

        public string ExamCode { get; set; } = string.Empty;

        public string ExamName { get; set; } = string.Empty;

        public string? SectionName { get; set; }

        public decimal GrandTotal { get; set; }

        public decimal MaximumMarks { get; set; }

        public decimal Percentage { get; set; }

        public string OverallGrade { get; set; } = string.Empty;

        public string FinalResult { get; set; } = string.Empty;

        public string ResultStatus { get; set; } = string.Empty;

        public DateTime? PublishedDate { get; set; }

        public List<StudentSubjectResultDto> Subjects { get; set; } = new();

        public int? ClassRank { get; set; }
    }
}