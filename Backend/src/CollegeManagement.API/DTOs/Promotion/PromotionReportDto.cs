namespace CollegeManagement.API.DTOs.Promotion
{
    public class PromotionReportDto
    {
        public int TotalStudents { get; set; }

        public int PromotedStudents { get; set; }

        public int PendingStudents { get; set; }

        public int RollbackStudents { get; set; }
    }
}