using System.Collections.Generic;

namespace CollegeManagement.API.DTOs.Staff
{
    public class StaffWorkloadResponseDto
    {
        public int StaffId { get; set; }
        public int FacultyId => StaffId;
        public string StaffName { get; set; } = string.Empty;
        public string FacultyName
        {
            get => StaffName;
            set => StaffName = value;
        }
        public string EmployeeId { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string Designation { get; set; } = string.Empty;
        public int TotalAssignedSubjects { get; set; }
        public int TotalSections { get; set; }
        public int WeeklyClasses { get; set; }
        public decimal TotalWorkloadHours { get; set; }
        public List<StaffSubjectAllocationResponseDto> Allocations { get; set; } = new List<StaffSubjectAllocationResponseDto>();
    }
}
