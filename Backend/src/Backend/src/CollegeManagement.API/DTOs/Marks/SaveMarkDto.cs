namespace CollegeManagement.API.DTOs.Marks
{
    public class SaveMarkDto
    {
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
        public int PassingMarks { get; set; }
    }
}