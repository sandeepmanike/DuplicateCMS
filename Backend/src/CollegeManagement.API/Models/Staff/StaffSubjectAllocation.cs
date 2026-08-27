using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CollegeManagement.API.Models;

namespace CollegeManagement.API.Models.Staff
{
    [Table("StaffSubjectAllocations")]
    public class StaffSubjectAllocation
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        [Required]
        [Column("StaffId")]
        public int StaffId { get; set; }

        [Required]
        [Column("SubjectId")]
        public int SubjectId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Navigation
        [ForeignKey(nameof(StaffId))]
        public virtual Staff? Staff { get; set; }

        [ForeignKey(nameof(SubjectId))]
        public virtual Subject? Subject { get; set; }

        // Unmapped in DB (ensures database table only contains Id, StaffId, SubjectId, CreatedAt, UpdatedAt)
        [NotMapped]
        public int? FacultyId { get => StaffId; set => StaffId = value ?? 0; }

        [NotMapped]
        public int? AcademicYearId { get; set; }

        [NotMapped]
        public int? SectionId { get; set; }

        [NotMapped]
        public bool IsActive { get; set; } = true;
    }
}
