namespace CollegeManagement.API.DTOs.Examination.Requests
{
    public class UpdateExamScheduleRequest
    {
        public DateTime ExamDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string Venue { get; set; } = string.Empty;
    }
}