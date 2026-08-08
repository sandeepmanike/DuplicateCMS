using System;
using CollegeManagement.API.Enums;

namespace CollegeManagement.API.DTOs.Attendance.Requests
{
    /// <summary>
    /// Request DTO for updating an existing attendance record.
    /// </summary>
    public class UpdateAttendanceRequest
    {
        /// <summary>
        /// Gets or sets the unique identifier of the attendance record.
        /// </summary>
        public int AttendanceId { get; set; }

        /// <summary>
        /// Gets or sets the date of the attendance.
        /// </summary>
        public DateTime AttendanceDate { get; set; }

        /// <summary>
        /// Gets or sets the student identifier.
        /// </summary>
        public int StudentId { get; set; }

        /// <summary>
        /// Gets or sets the faculty identifier.
        /// </summary>
        public int FacultyId { get; set; }

        /// <summary>
        /// Gets or sets the board identifier.
        /// </summary>
        public int BoardId { get; set; }

        /// <summary>
        /// Gets or sets the academic year identifier.
        /// </summary>
        public int AcademicYearId { get; set; }

        /// <summary>
        /// Gets or sets the academic level identifier.
        /// </summary>
        public int AcademicLevelId { get; set; }

        /// <summary>
        /// Gets or sets the group identifier.
        /// </summary>
        public int GroupId { get; set; }

        /// <summary>
        /// Gets or sets the section identifier.
        /// </summary>
        public int SectionId { get; set; }

        /// <summary>
        /// Gets or sets the subject identifier.
        /// </summary>
        public int SubjectId { get; set; }

        /// <summary>
        /// Gets or sets the attendance status.
        /// </summary>
        public AttendanceStatus Status { get; set; }

        /// <summary>
        /// Gets or sets any remarks or notes for the attendance.
        /// </summary>
        public string? Remarks { get; set; }
    }
}
