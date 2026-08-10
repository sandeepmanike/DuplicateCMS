namespace CollegeManagement.API.DTOs.Promotion
{
    public class PromotionHistoryDto
    {
        public int PromotionId { get; set; }

        public int StudentId { get; set; }

        public string StudentName { get; set; } = string.Empty;

        public string FromClass { get; set; } = string.Empty;

        public string ToClass { get; set; } = string.Empty;

        public DateTime PromotionDate { get; set; }

        public string PromotedBy { get; set; } = string.Empty;

        public string? Remarks { get; set; }
    }
}