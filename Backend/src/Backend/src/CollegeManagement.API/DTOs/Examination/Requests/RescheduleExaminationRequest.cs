namespace CollegeManagement.API.DTOs.Examination.Requests
{
    public class RescheduleExaminationRequest
    {
        public DateTime NewDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
}