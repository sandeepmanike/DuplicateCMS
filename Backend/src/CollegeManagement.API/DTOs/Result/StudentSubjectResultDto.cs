namespace CollegeManagement.API.DTOs.Result
{
    public class StudentSubjectResultDto
    {
        public int SubjectId { get; set; }

        public string SubjectName { get; set; } = string.Empty;

        public string SubjectCode { get; set; } = string.Empty;

        public decimal Theory { get; set; }

        public decimal Practical { get; set; }

        public decimal Internal { get; set; }

        public decimal TotalMarks { get; set; }

        public decimal MaximumMarks { get; set; }

        public string Grade { get; set; } = string.Empty;

        public string ResultStatus { get; set; } = string.Empty;

        public bool IsPublished { get; set; }
    }
}