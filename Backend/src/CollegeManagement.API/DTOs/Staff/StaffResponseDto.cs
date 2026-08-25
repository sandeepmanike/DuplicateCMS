using System;

namespace CollegeManagement.API.DTOs.Staff
{
    public class StaffResponseDto
    {
        public int Id { get; set; }
        public int StaffId => Id;
        public int FacultyId => Id;
        public string EmployeeId { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName => $"{FirstName} {LastName}".Trim();
        public string Gender { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string? Aadhaar { get; set; }
        public string Mobile { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? BloodGroup { get; set; }
        public string Qualification { get; set; } = string.Empty;
        public string Designation { get; set; } = string.Empty;
        public int? DesignationId { get; set; }
        public string StaffType { get; set; } = "Teaching";
        public string FacultyType => StaffType;
        public int? DepartmentId { get; set; }
        public string Department { get; set; } = string.Empty;
        public DateTime JoiningDate { get; set; }
        public decimal Experience { get; set; }
        public string Status { get; set; } = "Active";
        public string? PhotoPath { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
