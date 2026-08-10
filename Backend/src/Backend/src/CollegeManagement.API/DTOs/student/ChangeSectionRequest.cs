using System.ComponentModel.DataAnnotations;

namespace CollegeManagement.API.DTOs.Students
{
    public class ChangeSectionRequest
    {
        [Required]
        public string Section { get; set; } = string.Empty;
    }
}