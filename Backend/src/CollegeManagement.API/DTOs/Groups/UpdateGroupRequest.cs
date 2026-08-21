using System.ComponentModel.DataAnnotations;

namespace CollegeManagement.API.DTOs.Groups
{
    public class UpdateGroupRequest
    {
        [Range(1, int.MaxValue, ErrorMessage = "Valid BoardId is required")]
        public int BoardId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Valid AcademicYearId is required")]
        public int AcademicYearId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Valid AcademicLevelId is required")]
        public int AcademicLevelId { get; set; }

        [Required, MaxLength(100)]
        public string GroupName { get; set; } = string.Empty;

        [Required, MaxLength(30)]
        [RegularExpression(@"^[A-Za-z0-9_-]+$", ErrorMessage = "Group code can contain only letters, numbers, hyphen and underscore.")]
        public string GroupCode { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        public bool IsActive { get; set; }
        public List<int> ProgramIds { get; set; } = new();
    }
}
