using System;

namespace CollegeManagement.API.DTOs.Marks
{
    public class MarkResponseDto
    {
        public int MarkId { get; set; }
        public string Board { get; set; } = string.Empty;
        public int AcademicYearId { get; set; }
        public string AcademicLevel { get; set; } = string.Empty;
        public int GroupId { get; set; }
        public int SectionId { get; set; }
        public int ExaminationId { get; set; }
        public int SubjectId { get; set; }
        public int StudentId { get; set; }
        public string RollNo { get; set; } = string.Empty;
        public string StudentName { get; set; } = string.Empty;
        public int InternalMarks { get; set; }
        public int PracticalMarks { get; set; }
        public int TheoryMarks { get; set; }
        public int TotalMarks { get; set; }
        public int PassingMarks { get; set; }
        public bool IsPass => TotalMarks >= PassingMarks;
        public bool IsVerified { get; set; }
        public bool IsPublished { get; set; }
        public string? VerifiedBy { get; set; }
        public DateTime? VerifiedAt { get; set; }
        public DateTime? PublishedAt { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}