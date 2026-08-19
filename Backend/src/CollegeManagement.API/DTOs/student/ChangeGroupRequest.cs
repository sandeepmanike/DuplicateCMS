using System.ComponentModel.DataAnnotations;

namespace CollegeManagement.API.DTOs.Students
{
    public class ChangeGroupRequest
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int GroupId { get; set; }
    }
}