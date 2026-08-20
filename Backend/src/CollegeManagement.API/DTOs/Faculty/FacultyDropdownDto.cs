namespace CollegeManagement.API.DTOs.Faculty
{
    public class FacultyDropdownDto
    {
        public int Id { get; set; }
        public string EmployeeId { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Designation { get; set; } = string.Empty;
        public int? DesignationId { get; set; }
        public string FacultyType { get; set; } = "Teaching";
    }
}
