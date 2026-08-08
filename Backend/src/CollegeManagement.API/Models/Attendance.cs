using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CollegeManagement.API.Enums;
using FacultyEntity = CollegeManagement.API.Models.Faculty.Faculty;

namespace CollegeManagement.API.Models
{
    /// <summary>
    /// Represents an Attendance record in the College Management System.
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

        /// <summary>
        /// Gets or sets the date of the attendance.
        /// </summary>
        [Required]
        public DateTime AttendanceDate { get; set; }

        /// <summary>
        /// Gets or sets the student identifier.
        /// </summary>
        [Required]
        public int StudentId { get; set; }

        /// <summary>
        /// Gets or sets the faculty identifier.
        /// </summary>
        [Required]
        public int FacultyId { get; set; }

        /// <summary>
        /// Gets or sets the board identifier.
        /// </summary>
        [Required]
        public int BoardId { get; set; }

        /// <summary>
        /// Gets or sets the academic year identifier.
        /// </summary>
        [Required]
        public int AcademicYearId { get; set; }

        /// <summary>
        /// Gets or sets the academic level identifier.
        /// </summary>
        [Required]
        public int AcademicLevelId { get; set; }

        /// <summary>
        /// Gets or sets the group identifier.
        /// </summary>
        [Required]
        public int GroupId { get; set; }

        /// <summary>
        /// Gets or sets the section identifier.
        /// </summary>
        [Required]
        public int SectionId { get; set; }

        /// <summary>
        /// Gets or sets the subject identifier.
        /// </summary>
        [Required]
        public int SubjectId { get; set; }

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

        /// <summary>
        /// Gets or sets the associated faculty member.
        /// </summary>
        [ForeignKey(nameof(FacultyId))]
        public virtual FacultyEntity Faculty { get; set; } = null!;

        /// <summary>
        /// Gets or sets the associated board.
        /// </summary>
        [ForeignKey(nameof(BoardId))]
        public virtual Board Board { get; set; } = null!;

        /// <summary>
        /// Gets or sets the associated academic year.
        /// </summary>
        [ForeignKey(nameof(AcademicYearId))]
        public virtual AcademicYear AcademicYear { get; set; } = null!;

        /// <summary>
        /// Gets or sets the associated academic level.
        /// </summary>
        [ForeignKey(nameof(AcademicLevelId))]
        public virtual AcademicLevel AcademicLevel { get; set; } = null!;

        /// <summary>
        /// Gets or sets the associated group.
        /// </summary>
        [ForeignKey(nameof(GroupId))]
        public virtual Group Group { get; set; } = null!;

        /// <summary>
        /// Gets or sets the associated section.
        /// </summary>
        [ForeignKey(nameof(SectionId))]
        public virtual Section Section { get; set; } = null!;

        /// <summary>
        /// Gets or sets the associated subject.
        /// </summary>
        [ForeignKey(nameof(SubjectId))]
        public virtual Subject Subject { get; set; } = null!;

        #endregion
    }
}
