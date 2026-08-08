namespace CollegeManagement.API.Models.Promotion
{
    public class PromotionReport
    {
        public int TotalStudents { get; set; }

        public int EligibleStudents { get; set; }

        public int PromotedStudents { get; set; }

        public int PendingStudents { get; set; }

        public int RollbackStudents { get; set; }
    }
}