using System;
using System.Collections.Generic;
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

        [Column("StaffId")]
        public int StaffId { get; set; }

        [Column("FacultyId")]
        public int? FacultyId { get; set; }

        [Required]
        public int SubjectId { get; set; }

        public int? AcademicYearId { get; set; }

        public int? SectionId { get; set; }

        [Column("MaxWeeklyHours")]
        public int MaxWeeklyHours { get; set; } = 0;

        [NotMapped]
        public int? WeeklyHours
        {
            get => MaxWeeklyHours;
            set => MaxWeeklyHours = value ?? 0;
        }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Navigation
        [ForeignKey(nameof(StaffId))]
        public virtual Staff? Staff { get; set; }

        [ForeignKey(nameof(SubjectId))]
        public virtual Subject? Subject { get; set; }

        [ForeignKey(nameof(AcademicYearId))]
        public virtual AcademicYear? AcademicYear { get; set; }

        [ForeignKey(nameof(SectionId))]
        public virtual Section? Section { get; set; }
    }
}
