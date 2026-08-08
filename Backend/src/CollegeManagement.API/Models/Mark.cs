using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CollegeManagement.API.Models
{
    [Table("Marks")]
    public class Mark
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MarkId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Board { get; set; } = string.Empty;

        [Required]
        public int AcademicYearId { get; set; }

        [Required]
        [MaxLength(50)]
        public string AcademicLevel { get; set; } = string.Empty;

        [Required]
        public int GroupId { get; set; }

        [Required]
        public int SectionId { get; set; }

        [Required]
        public int ExaminationId { get; set; }

        [ForeignKey("ExaminationId")]
        public Examination? Examination { get; set; }

        [Required]
        public int SubjectId { get; set; }

        [Required]
        public int StudentId { get; set; }

        [Required]
        [MaxLength(50)]
        public string RollNo { get; set; } = string.Empty;

        [Required]
        [MaxLength(150)]
        public string StudentName { get; set; } = string.Empty;

        public int InternalMarks { get; set; }
        public int PracticalMarks { get; set; }
        public int TheoryMarks { get; set; }
        public int TotalMarks { get; set; }
        public int PassingMarks { get; set; }

        public bool IsVerified { get; set; } = false;
        public bool IsPublished { get; set; } = false;

        [MaxLength(100)]
        public string? VerifiedBy { get; set; }

        public DateTime? VerifiedAt { get; set; }
        public DateTime? PublishedAt { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}