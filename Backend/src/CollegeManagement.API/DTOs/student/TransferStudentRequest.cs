using System.ComponentModel.DataAnnotations;

namespace CollegeManagement.API.DTOs.Students
{
    public class TransferStudentRequest
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int BoardId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int AcademicYearId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int AcademicLevelId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int GroupId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int SectionId { get; set; }
    }
}