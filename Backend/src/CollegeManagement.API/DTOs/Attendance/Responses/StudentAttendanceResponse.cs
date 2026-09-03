using CollegeManagement.API.Enums;

namespace CollegeManagement.API.DTOs.Attendance.Responses
{
    /// <summary>
    /// Response DTO representing a student available for marking attendance, including their current attendance status if already recorded.
    /// </summary>
    public class StudentAttendanceResponse
    {
        /// <summary>
        /// Gets or sets the student identifier.
        /// </summary>
        public int StudentId { get; set; }

        /// <summary>
        /// Gets or sets the student's admission number.
        /// </summary>
        public string AdmissionNumber { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the student's roll number.
        /// </summary>
        public string RollNumber { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the full name of the student.
        /// </summary>
        public string StudentName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the attendance status of the student for the selected date. 
        /// Null indicates attendance has not yet been marked.
        /// </summary>
        public AttendanceStatus? Status { get; set; }

        /// <summary>
        /// Gets or sets optional remarks or notes for this student's attendance.
        /// </summary>
        public string? Remarks { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether attendance has been marked for the student.
        /// </summary>
        public bool IsAttendanceMarked { get; set; }

        public StudentAttendanceSession? Session { get; set; }
        
        public int? AttendanceId { get; set; }

        public string? ModifiedByUserName { get; set; }

        public DateTime? ModifiedAt { get; set; }
    }
}
