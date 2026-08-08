namespace CollegeManagement.API.DTOs.Assignment
{
    public class AssignmentResponseDto
    {
        public int AssignmentId { get; set; }

        public string Title { get; set; } = string.Empty;

        public int SubjectId { get; set; }

        public int FacultyId { get; set; }

        public int AcademicYearId { get; set; }

        public string AcademicLevel { get; set; } = string.Empty;

        public string? Description { get; set; }

        public DateOnly DueDate { get; set; }

        public string? Attachment { get; set; }

        public int MaximumMarks { get; set; }
    }
}