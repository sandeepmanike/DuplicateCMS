using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CollegeManagement.API.Models.Faculty
{
    [Table("FacultySubjectAllocations")]
    public class FacultySubjectAllocation
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int FacultyId { get; set; }

        [Required]
        public int BoardId { get; set; }

        [Required]
        public int AcademicLevelId { get; set; }

        [Required]
        public int AcademicYearId { get; set; }

        [Required]
        public int GroupId { get; set; }

        [Required]
        public int SectionId { get; set; }

        [Required]
        public int SubjectId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Navigation Properties
        [ForeignKey(nameof(FacultyId))]
        public virtual Faculty Faculty { get; set; } = null!;

        [ForeignKey(nameof(BoardId))]
        public virtual Board Board { get; set; } = null!;

        [ForeignKey(nameof(AcademicLevelId))]
        public virtual AcademicLevel AcademicLevel { get; set; } = null!;

        [ForeignKey(nameof(AcademicYearId))]
        public virtual AcademicYear AcademicYear { get; set; } = null!;

        [ForeignKey(nameof(GroupId))]
        public virtual Group Group { get; set; } = null!;

        [ForeignKey(nameof(SectionId))]
        public virtual Section Section { get; set; } = null!;

        [ForeignKey(nameof(SubjectId))]
        public virtual Subject Subject { get; set; } = null!;
    }
}
