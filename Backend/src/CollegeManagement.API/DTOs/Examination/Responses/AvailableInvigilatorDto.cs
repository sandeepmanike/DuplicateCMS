using System;

namespace CollegeManagement.API.DTOs.Examination.Responses
{
    public class AvailableInvigilatorDto
    {
        public int FacultyId { get; set; }
        public string EmployeeId { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Designation { get; set; } = string.Empty;
        public string FacultyType { get; set; } = "Teaching";
        public bool IsAvailable { get; set; } = true;
    }
}
