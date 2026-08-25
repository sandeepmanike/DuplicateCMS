namespace CollegeManagement.API.DTOs.Students
{
    public class TransferStudentRequest
    {
        public int BoardId { get; set; }
        public int AcademicYearId { get; set; }
        public int AcademicLevelId { get; set; }
        public int GroupId { get; set; }
        public int SectionId { get; set; }
        public string? Remarks { get; set; }
    }
}
