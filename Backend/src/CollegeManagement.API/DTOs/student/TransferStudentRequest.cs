namespace CollegeManagement.API.DTOs.Students
{
    public class TransferStudentRequest
    {
        public string Board { get; set; } = string.Empty;

        public int AcademicYearId { get; set; }

        public string AcademicLevel { get; set; } = string.Empty;

        public int GroupId { get; set; }

        public string Section { get; set; } = string.Empty;
    }
}