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
        /// Gets or sets the attendance status.
        /// </summary>
        public AttendanceStatus Status { get; set; }

        /// <summary>
        /// Gets or sets any remarks or notes for the attendance.
        /// </summary>
        public string? Remarks { get; set; }
    }
}
