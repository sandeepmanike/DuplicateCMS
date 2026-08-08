namespace CollegeManagement.API.DTOs.Students
{
    public class StudentDashboardDto
    {
        // ==========================
        // Basic Information
        // ==========================

        public int StudentId { get; set; }

        public string StudentName { get; set; } = string.Empty;

        public string AdmissionNo { get; set; } = string.Empty;

        public string RollNo { get; set; } = string.Empty;

        public string? Photo { get; set; }

        public string Board { get; set; } = string.Empty;

        public string AcademicYearName { get; set; } = string.Empty;

        public string AcademicLevel { get; set; } = string.Empty;

        public string GroupName { get; set; } = string.Empty;

        public string Section { get; set; } = string.Empty;

        // ==========================
        // Attendance
        // ==========================

        public decimal AttendancePercentage { get; set; }

        // ==========================
        // Fee Summary
        // ==========================

        public decimal FeeAmount { get; set; }

        public decimal FeePaid { get; set; }

        public decimal FeeDue { get; set; }

        public decimal ScholarshipAmount { get; set; }

        public string? FeeStatus { get; set; }

        // ==========================
        // Academic Performance
        // ==========================

        public string? PerformanceGrade { get; set; }

        public decimal? CGPA { get; set; }

        public int? Rank { get; set; }

        // ==========================
        // Subjects
        // ==========================

        public int TotalSubjects { get; set; }

        public int CompletedSubjects { get; set; }

        public int PendingSubjects { get; set; }

        // ==========================
        // Status
        // ==========================

        public bool IsActive { get; set; }

        public DateTime? LastLogin { get; set; }
    }
}