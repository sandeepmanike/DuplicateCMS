using System.ComponentModel.DataAnnotations;

namespace CollegeManagement.API.DTOs.Promotion
{
    public class PromotionRequestDto
    {
        [Required]
        public List<int> StudentIds { get; set; } = new();

        [Required]
        public int NewClassId { get; set; }

        [Required]
        public int AcademicYearId { get; set; }

        public string? Remarks { get; set; }
    }
}