    using System.ComponentModel.DataAnnotations;

    namespace CollegeManagement.API.DTOs.Promotion
    {
        public class GroupAllocationDto
        {
            [Required]
            public List<int> StudentIds { get; set; } = new();

            [Required]
            public int GroupId { get; set; }
        }
    }