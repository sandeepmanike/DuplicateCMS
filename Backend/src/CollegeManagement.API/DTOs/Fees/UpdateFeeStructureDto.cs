using System.ComponentModel.DataAnnotations;

namespace CollegeManagement.API.DTOs.Fee
{
    public class UpdateFeeStructureDto
    {
        [Required]
        public int BoardId { get; set; }

        [Required]
        public int AcademicYearId { get; set; }

        [Required]
        public int AcademicLevelId { get; set; }

        [Required]
        public int GroupId { get; set; }

        [Required]
        [MinLength(1)]
        public List<UpdateFeeStructureItemDto> Items { get; set; }
            = new();

        public bool IsActive { get; set; } = true;
    }
}