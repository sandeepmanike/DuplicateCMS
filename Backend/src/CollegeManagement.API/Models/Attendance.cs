using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CollegeManagement.API.Enums;

namespace CollegeManagement.API.Models
{
    /// <summary>
    /// Represents an Attendance detail record in the College Management System.
    /// </summary>
    [Table("Attendances")]
    public class Attendance
    {
        /// <summary>
        /// Gets or sets the unique identifier of the attendance record.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AttendanceId { get; set; }

        [NotMapped]
        public int? AttendanceSessionId { get; set; }

        /// <summary>
        /// Gets or sets the student identifier.
        /// </summary>
        [Required]
        public int StudentId { get; set; }

        public int? SubjectId { get; set; }

        public int? GroupId { get; set; }

        public int? SectionId { get; set; }

        public int? AcademicYearId { get; set; }

        public int? AcademicLevelId { get; set; }

        public int? BoardId { get; set; }

        public int? FacultyId { get; set; }

        public DateTime AttendanceDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Gets or sets the status of the attendance (1 = Present, 2 = Absent, 3 = Late, 4 = Leave).
        /// </summary>
        [Required]
        public AttendanceStatus Status { get; set; }

        /// <summary>
        /// Gets or sets any remarks or notes for the attendance.
        /// </summary>
        [MaxLength(500)]
        public string? Remarks { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the attendance record is active.
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Gets or sets the record creation date.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Gets or sets the last updated date.
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        #region Navigation Properties

        /// <summary>
        /// Gets or sets the associated student.
        /// </summary>
        [ForeignKey(nameof(StudentId))]
        public virtual Student Student { get; set; } = null!;

        [NotMapped]
        public virtual AttendanceSession? AttendanceSession { get; set; }

        #endregion
    }
}
