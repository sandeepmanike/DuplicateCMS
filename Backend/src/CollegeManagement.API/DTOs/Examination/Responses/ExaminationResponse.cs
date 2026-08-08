using System;
using System.Collections.Generic;

namespace CollegeManagement.API.DTOs.Examination.Responses
{
    public class ExaminationResponse
    {
        public int ExaminationId { get; set; }
        public string ExamCode { get; set; } = string.Empty;
        public string ExamName { get; set; } = string.Empty;
        public string BoardName { get; set; } = string.Empty;
        public string GroupName { get; set; } = string.Empty;
        public string ExamType { get; set; } = string.Empty;
        public string AcademicYear { get; set; } = string.Empty;
        public string AcademicLevel { get; set; } = string.Empty;
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public int TotalSubjects { get; set; }
        public decimal TotalMaxMarks { get; set; }
        public decimal TotalPassMarks { get; set; }
        public int ExamDurationMinutes { get; set; }
        public List<ExamScheduleResponse> Schedules { get; set; } = new();
    }
}