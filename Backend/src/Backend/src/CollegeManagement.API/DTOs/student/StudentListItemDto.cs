namespace CollegeManagement.API.DTOs.Students
{
    public class StudentListItemDto
    {
        // ==========================
        // Basic Information
        // ==========================

        public int StudentId { get; set; }

        public string AdmissionNo { get; set; } = string.Empty;

        public string RollNo { get; set; } = string.Empty;

        public string StudentName { get; set; } = string.Empty;

        public string? Photo { get; set; }

        public string Gender { get; set; } = string.Empty;

        // ==========================
        // Academic Information
        // ==========================

        public string Board { get; set; } = string.Empty;

        public int AcademicYearId { get; set; }

        public string AcademicYearName { get; set; } = string.Empty;

        public string AcademicLevel { get; set; } = string.Empty;

        public int GroupId { get; set; }

        public string GroupName { get; set; } = string.Empty;

        public string Section { get; set; } = string.Empty;

        public DateTime AdmissionDate { get; set; }

        // ==========================
        // Contact Information
        // ==========================

        public string Email { get; set; } = string.Empty;

        public string MobileNumber { get; set; } = string.Empty;

        public string? FatherName { get; set; }

        public string? FatherMobile { get; set; }

        // ==========================
        // Fee & Attendance
        // ==========================

        public decimal FeeAmount { get; set; }

        public decimal FeePaid { get; set; }

        public decimal ScholarshipAmount { get; set; }

        public string? FeeStatus { get; set; }

        public decimal AttendancePercentage { get; set; }

        public string? PerformanceGrade { get; set; }

        // ==========================
        // Status
        // ==========================

        public bool IsActive { get; set; }

        public string Status { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}