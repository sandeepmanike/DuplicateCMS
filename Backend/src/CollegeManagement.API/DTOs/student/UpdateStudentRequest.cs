using System.ComponentModel.DataAnnotations;

namespace CollegeManagement.API.DTOs.Students
{
    public class UpdateStudentRequest
    {
        [Required]
        [MaxLength(50)]
        public string AdmissionNo { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string RollNo { get; set; } = string.Empty;

        [Required]
        [MaxLength(150)]
        public string StudentName { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Photo { get; set; }


        // =========================================================
        // PERSONAL
        // =========================================================

        [Required]
        [MaxLength(20)]
        public string Gender { get; set; } = string.Empty;

        [Required]
        public DateTime DateOfBirth { get; set; }

        [MaxLength(10)]
        public string? BloodGroup { get; set; }

        [EmailAddress]
        [MaxLength(150)]
        public string? Email { get; set; }

        [MaxLength(20)]
        public string? MobileNumber { get; set; }

        [RegularExpression(
            @"^[0-9]{12}$",
            ErrorMessage = "Aadhaar number must be exactly 12 digits.")]
        public string? AadhaarNumber { get; set; }

        [MaxLength(50)]
        public string? Nationality { get; set; }

        [MaxLength(50)]
        public string? Religion { get; set; }

        [MaxLength(50)]
        public string? Category { get; set; }


        // =========================================================
        // ADDRESS
        // =========================================================

        [MaxLength(500)]
        public string? Address { get; set; }

        [MaxLength(100)]
        public string? City { get; set; }

        [MaxLength(100)]
        public string? District { get; set; }

        [MaxLength(100)]
        public string? State { get; set; }

        [MaxLength(20)]
        public string? Pincode { get; set; }


        // =========================================================
        // ACADEMIC REFERENCES
        // =========================================================

        [Required]
        public int BoardId { get; set; }

        [Required]
        public int AcademicYearId { get; set; }

        [Required]
        public int AcademicLevelId { get; set; }

        [Required]
        public int GroupId { get; set; }

        [Required]
        public int SectionId { get; set; }


        // =========================================================
        // ADMISSION
        // =========================================================

        [Required]
        public DateTime AdmissionDate { get; set; }

        [MaxLength(50)]
        public string? AdmissionType { get; set; }

        [MaxLength(50)]
        public string? AdmissionQuota { get; set; }

        [MaxLength(50)]
        public string? Medium { get; set; }

        [MaxLength(50)]
        public string? SecondLanguage { get; set; }


        // =========================================================
        // PREVIOUS EDUCATION
        // =========================================================

        [MaxLength(200)]
        public string? PreviousSchool { get; set; }

        [MaxLength(100)]
        public string? PreviousHallTicketNumber { get; set; }

        [MaxLength(100)]
        public string? PreviousBoard { get; set; }

        public int? PreviousYearOfPassing { get; set; }

        [Range(0, 100)]
        public decimal? PreviousPercentage { get; set; }


        // =========================================================
        // SCHOLARSHIP
        // =========================================================

        [MaxLength(50)]
        public string? StudentCategory { get; set; }

        [MaxLength(50)]
        public string? ScholarshipStatus { get; set; }

        public decimal? ScholarshipAmount { get; set; }


        // =========================================================
        // PARENT
        // =========================================================

        [MaxLength(150)]
        public string? FatherName { get; set; }

        [MaxLength(100)]
        public string? FatherOccupation { get; set; }

        [MaxLength(20)]
        public string? FatherMobile { get; set; }

        [EmailAddress]
        [MaxLength(150)]
        public string? FatherEmail { get; set; }

        [MaxLength(150)]
        public string? MotherName { get; set; }

        [MaxLength(100)]
        public string? MotherOccupation { get; set; }

        [MaxLength(20)]
        public string? MotherMobile { get; set; }

        [EmailAddress]
        [MaxLength(150)]
        public string? MotherEmail { get; set; }

        [MaxLength(150)]
        public string? GuardianName { get; set; }

        [MaxLength(20)]
        public string? GuardianMobile { get; set; }

        [EmailAddress]
        [MaxLength(150)]
        public string? GuardianEmail { get; set; }

        public decimal? AnnualIncome { get; set; }


        // =========================================================
        // FEES
        // =========================================================

        public decimal FeeAmount { get; set; }

        public decimal FeePaid { get; set; }

        [MaxLength(30)]
        public string? FeeStatus { get; set; }


        // =========================================================
        // PERFORMANCE
        // =========================================================

        public decimal AttendancePercentage { get; set; }

        [MaxLength(20)]
        public string? PerformanceGrade { get; set; }

        public decimal? CGPA { get; set; }

        public int? Rank { get; set; }


        // =========================================================
        // DOCUMENTS
        // =========================================================

        [MaxLength(500)]
        public string? BirthCertificate { get; set; }

        [MaxLength(500)]
        public string? TransferCertificate { get; set; }

        [MaxLength(500)]
        public string? StudyCertificate { get; set; }

        [MaxLength(500)]
        public string? AadhaarDocument { get; set; }

        [MaxLength(500)]
        public string? CommunityCertificate { get; set; }

        [MaxLength(500)]
        public string? IncomeCertificate { get; set; }

        [MaxLength(500)]
        public string? CasteCertificate { get; set; }

        [MaxLength(500)]
        public string? TenthCertificate { get; set; }

        [MaxLength(500)]
        public string? MarksMemo { get; set; }


        // =========================================================
        // REMARKS
        // =========================================================

        [MaxLength(1000)]
        public string? Remarks { get; set; }

        public bool IsActive { get; set; } = true;
    }
}