namespace CollegeManagement.API.DTOs.Assignment
{
    public class AssignmentSubmissionResponseDto
    {
        public int AssignmentSubmissionId { get; set; }

        public int AssignmentId { get; set; }

        public string StudentName { get; set; } = string.Empty;

        public string SubmissionFile { get; set; } = string.Empty;

        public DateTime SubmittedAt { get; set; }
    }
}