using System.ComponentModel.DataAnnotations;

namespace CollegeManagement.API.DTOs.Sections
{
    public class UpdateSectionRequest
    {
        [Required(ErrorMessage = "Board is required.")]
        [MaxLength(100, ErrorMessage = "Board name cannot exceed 100 characters.")]
        public string Board { get; set; } = string.Empty;

        [Required(ErrorMessage = "Academic Year ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Valid Academic Year ID is required.")]
        public int AcademicYearId { get; set; }

        [Required(ErrorMessage = "Group is required.")]
        [MaxLength(100, ErrorMessage = "Group name cannot exceed 100 characters.")]
        public string Group { get; set; } = string.Empty;

        [Required(ErrorMessage = "Academic Level is required.")]
        [MaxLength(50, ErrorMessage = "Academic Level cannot exceed 50 characters.")]
        public string AcademicLevel { get; set; } = string.Empty;

        [Required(ErrorMessage = "Section Name is required.")]
        [MaxLength(50, ErrorMessage = "Section Name cannot exceed 50 characters.")]
        public string SectionName { get; set; } = string.Empty;

        [MaxLength(50, ErrorMessage = "Room Number cannot exceed 50 characters.")]
        public string? RoomNumber { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Valid Class Teacher ID is required.")]
        public int? ClassTeacherId { get; set; }

        [Required(ErrorMessage = "Maximum Strength is required.")]
        [Range(1, 1000, ErrorMessage = "Maximum Strength must be between 1 and 1000.")]
        public int MaximumStrength { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
