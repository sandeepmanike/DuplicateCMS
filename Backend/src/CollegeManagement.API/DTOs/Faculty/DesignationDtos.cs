using System;
using System.ComponentModel.DataAnnotations;

namespace CollegeManagement.API.DTOs.Faculty
{
    public class DesignationResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CreateDesignationDto
    {
        [Required(ErrorMessage = "Designation name is required.")]
        [StringLength(100, ErrorMessage = "Designation name cannot exceed 100 characters.")]
        public string Name { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;
    }

    public class UpdateDesignationDto
    {
        [Required(ErrorMessage = "Designation name is required.")]
        [StringLength(100, ErrorMessage = "Designation name cannot exceed 100 characters.")]
        public string Name { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;
    }
}
