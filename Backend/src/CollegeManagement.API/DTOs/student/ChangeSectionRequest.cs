using System.ComponentModel.DataAnnotations;

namespace CollegeManagement.API.DTOs.Students
{
    public class ChangeSectionRequest
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int SectionId { get; set; }
    }
}