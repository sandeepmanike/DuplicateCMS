using System.ComponentModel.DataAnnotations;

namespace CollegeManagement.API.DTOs.Subject
{
    public class UpdateSubjectDto
    {
        [Range(1, int.MaxValue)] public int BoardId { get; set; }
        [Range(1, int.MaxValue)] public int AcademicYearId { get; set; }
        [Range(1, int.MaxValue)] public int AcademicLevelId { get; set; }
        [Range(1, int.MaxValue)] public int GroupId { get; set; }
        [Required, MaxLength(150)] public string SubjectName { get; set; } = string.Empty;
        [Required, MaxLength(50)] public string SubjectCode { get; set; } = string.Empty;
        [Required] public string SubjectType { get; set; } = string.Empty;
        public bool Theory { get; set; }
        public bool Practical { get; set; }
        public bool Language { get; set; }
        public bool Elective { get; set; }
        public int InternalMarks { get; set; }
        public int PracticalMarks { get; set; }
        public int ExternalMarks { get; set; }
        public int TotalMarks { get; set; }
        public int PassingMarks { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
