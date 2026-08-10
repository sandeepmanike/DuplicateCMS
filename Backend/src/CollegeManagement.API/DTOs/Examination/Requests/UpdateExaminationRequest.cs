namespace CollegeManagement.API.DTOs.Examination.Requests
{
    public class UpdateExaminationRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int TotalMarks { get; set; }
        public int PassingMarks { get; set; }
    }
}