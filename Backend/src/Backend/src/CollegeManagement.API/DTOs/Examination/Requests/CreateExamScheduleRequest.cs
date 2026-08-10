using System;

namespace CollegeManagement.API.DTOs.Examination.Requests
{
    public class CreateExamScheduleRequest
    {
        public int ExaminationId { get; set; }
        public int SubjectId { get; set; }
        public DateOnly ExamDate { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public string RoomNumber { get; set; } = string.Empty;
        public string InvigilatorName { get; set; } = string.Empty;
    }
}