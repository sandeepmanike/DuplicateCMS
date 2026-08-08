namespace CollegeManagement.API.DTOs.Result
{
    public class RankListDto
    {
        public int Rank { get; set; }

        public int StudentId { get; set; }

        public string StudentName { get; set; } = string.Empty;

        public string RollNumber { get; set; } = string.Empty;

        public string GroupName { get; set; } = string.Empty;

        public string ExamName { get; set; } = string.Empty;

        public decimal TotalMarks { get; set; }

        public decimal Percentage { get; set; }

        public string Grade { get; set; } = string.Empty;
    }
}