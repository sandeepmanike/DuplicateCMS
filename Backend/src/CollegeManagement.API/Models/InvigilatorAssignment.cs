using System;

namespace CollegeManagement.API.Models
{
    public class InvigilatorAssignment
    {
        public int InvigilatorAssignmentId { get; set; }
        public int ExamScheduleId { get; set; }
        public int InvigilatorId { get; set; }
        public string HallNumber { get; set; } = string.Empty;
        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;

        public ExamSchedule? ExamSchedule { get; set; }
        public CollegeManagement.API.Models.User? Invigilator { get; set; } // Fully qualify User/Faculty to avoid namespace collision
    }
}