namespace CollegeManagement.API.DTOs.Students
{
    public class StudentDashboardDto
    {
        // =========================================================
        // BASIC
        // =========================================================

        public int StudentId { get; set; }

        public string StudentName { get; set; } = string.Empty;

        public string AdmissionNo { get; set; } = string.Empty;

        public string RollNo { get; set; } = string.Empty;

        public string? Photo { get; set; }


        // =========================================================
        // ACADEMIC IDS
        // =========================================================

        public int BoardId { get; set; }

        public int AcademicYearId { get; set; }

        public int AcademicLevelId { get; set; }

        public int GroupId { get; set; }

        public int SectionId { get; set; }


        // =========================================================
        // ACADEMIC NAMES
        // =========================================================

        public string? BoardName { get; set; }

        public string? AcademicYearName { get; set; }

        public string? AcademicLevelName { get; set; }

        public string? GroupName { get; set; }

        public string? SectionName { get; set; }


        // =========================================================
        // ATTENDANCE
        // =========================================================

        public decimal AttendancePercentage { get; set; }


        // =========================================================
        // FEES
        // =========================================================

        public decimal FeeAmount { get; set; }

        public decimal FeePaid { get; set; }

        public decimal FeeDue { get; set; }

        public decimal? ScholarshipAmount { get; set; }

        public string? FeeStatus { get; set; }


        // =========================================================
        // PERFORMANCE
        // =========================================================

        public string? PerformanceGrade { get; set; }

        public decimal? CGPA { get; set; }

        public int? Rank { get; set; }


        // =========================================================
        // SUBJECT SUMMARY
        // =========================================================

        public int TotalSubjects { get; set; }

        public int CompletedSubjects { get; set; }

        public int PendingSubjects { get; set; }


        // =========================================================
        // STATUS
        // =========================================================

        public bool IsActive { get; set; }

        public string Status { get; set; } = string.Empty;

        public DateTime? LastLogin { get; set; }
    }
}