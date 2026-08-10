using System.ComponentModel.DataAnnotations;

namespace CollegeManagement.API.DTOs.Promotion
{
    public class SectionAllocationDto
    {
        [Required]
        public List<int> StudentIds { get; set; } = new();

        [Required]
        public int SectionId { get; set; }
    }
}