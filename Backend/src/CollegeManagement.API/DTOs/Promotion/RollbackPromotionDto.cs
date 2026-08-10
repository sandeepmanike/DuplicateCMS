using System.ComponentModel.DataAnnotations;

namespace CollegeManagement.API.DTOs.Promotion
{
    public class RollbackPromotionDto
    {
        [Required]
        public int PromotionId { get; set; }

        public string? Remarks { get; set; }
    }
}