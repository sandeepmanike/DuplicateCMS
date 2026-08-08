using System;

namespace CollegeManagement.API.DTOs.Examination.Responses
{
    public class ExamScheduleResponse
    {
        public int ExamScheduleId { get; set; }
        public int ExaminationId { get; set; }
        public int SubjectId { get; set; }
        public string SubjectName { get; set; } = string.Empty;
        public DateOnly ExamDate { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public string RoomNumber { get; set; } = string.Empty;
        public string InvigilatorName { get; set; } = string.Empty;
        public string Status { get; set; } = "Scheduled";
    }
}