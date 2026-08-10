using System.ComponentModel.DataAnnotations;

namespace CollegeManagement.API.DTOs.Students
{
    public class ChangeGroupRequest
    {
        [Required]
        public int GroupId { get; set; }
    }
}