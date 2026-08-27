using System;
using System.ComponentModel.DataAnnotations;

namespace CollegeManagement.API.DTOs.Staff
{
    public class AssignStaffSubjectDto
    {
        [Required(ErrorMessage = "Staff ID is required.")]
        public int StaffId { get; set; }

        [Required(ErrorMessage = "Subject ID is required.")]
        public int SubjectId { get; set; }
    }

    public class UpdateStaffSubjectAllocationDto
    {
        [Required(ErrorMessage = "Subject ID is required.")]
        public int SubjectId { get; set; }
    }

    public class StaffSubjectAllocationResponseDto
    {
        public int Id { get; set; }
        public int StaffId { get; set; }
        public int FacultyId => StaffId;
        public string StaffName { get; set; } = string.Empty;
        public string FacultyName
        {
            get => StaffName;
            set => StaffName = value;
        }
        public int SubjectId { get; set; }
        public string SubjectCode { get; set; } = string.Empty;
        public string SubjectName { get; set; } = string.Empty;
        public string BoardName { get; set; } = string.Empty;
        public string GroupName { get; set; } = string.Empty;
        public string AcademicLevelName { get; set; } = string.Empty;
        public string? SectionName { get; set; }
        public string? Section { get; set; }
        public string Status { get; set; } = "Allocated";
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
