using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CollegeManagement.API.Models
{
    [Table("Assignments")]
    public class Assignment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AssignmentId { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public int SubjectId { get; set; }

        [Required]
        public int FacultyId { get; set; }

        [Required]
        public int AcademicYearId { get; set; }

        [Required]
        [MaxLength(50)]
        public string AcademicLevel { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        public DateOnly DueDate { get; set; }

        [Required]
        [MaxLength(500)]
        public string Attachment { get; set; } = string.Empty;

        public int MaximumMarks { get; set; }

        public Subject? Subject { get; set; }

        public AcademicYear? AcademicYear { get; set; }

        public CollegeManagement.API.Models.Faculty.Faculty? Faculty { get; set; }
    }
}