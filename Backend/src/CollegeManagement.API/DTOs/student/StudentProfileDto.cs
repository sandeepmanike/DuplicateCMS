namespace CollegeManagement.API.DTOs.Students
{
    public class StudentProfileDto
    {
        // ==========================
        // Basic Information
        // ==========================

        public int StudentId { get; set; }

        public string AdmissionNo { get; set; } = string.Empty;

        public string RollNo { get; set; } = string.Empty;

        public string StudentName { get; set; } = string.Empty;

        public string? Photo { get; set; }

        // ==========================
        // Personal Information
        // ==========================

        public string Gender { get; set; } = string.Empty;

        public DateTime DateOfBirth { get; set; }

        public string? BloodGroup { get; set; }

        public string Email { get; set; } = string.Empty;

        public string MobileNumber { get; set; } = string.Empty;

        public string? AadhaarNumber { get; set; }

        public string? Address { get; set; }

        // ==========================
        // Academic Information
        // ==========================

        public string Board { get; set; } = string.Empty;

        public string AcademicYearName { get; set; } = string.Empty;

        public string AcademicLevel { get; set; } = string.Empty;

        public string GroupName { get; set; } = string.Empty;

        public string Section { get; set; } = string.Empty;

        public DateTime AdmissionDate { get; set; }

        public string? AdmissionType { get; set; }

        public string? Medium { get; set; }

        public string? PreviousSchool { get; set; }

        public string? PreviousHallTicketNumber { get; set; }

        public string? StudentCategory { get; set; }

        public string? ScholarshipStatus { get; set; }

        // ==========================
        // Parent Details
        // ==========================

        public string? FatherName { get; set; }

        public string? FatherMobile { get; set; }

        public string? MotherName { get; set; }

        public string? MotherMobile { get; set; }

        public string? GuardianName { get; set; }

        public string? GuardianMobile { get; set; }

        // ==========================
        // Fee Information
        // ==========================

        public decimal FeeAmount { get; set; }

        public decimal FeePaid { get; set; }

        public decimal ScholarshipAmount { get; set; }

        public string? FeeStatus { get; set; }

        // ==========================
        // Attendance & Performance
        // ==========================

        public decimal AttendancePercentage { get; set; }

        public string? PerformanceGrade { get; set; }

        public decimal? CGPA { get; set; }

        public int? Rank { get; set; }

        public string? Remarks { get; set; }

        // ==========================
        // Status
        // ==========================

        public bool IsActive { get; set; }

        public string Status { get; set; } = string.Empty;
    }
}