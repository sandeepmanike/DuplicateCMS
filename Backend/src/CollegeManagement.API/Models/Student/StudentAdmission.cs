using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CollegeManagement.API.Models
{
    [Table("StudentAdmissions")]
    public class StudentAdmission
    {
        // =========================================================
        // PRIMARY KEY
        // =========================================================

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AdmissionId { get; set; }


        // =========================================================
        // ADMISSION DETAILS
        // =========================================================

        [Required]
        [MaxLength(50)]
        public string AdmissionNo { get; set; } = string.Empty;

        public DateTime AdmissionDate { get; set; }

        [MaxLength(50)]
        public string? AdmissionQuota { get; set; }


        // =========================================================
        // STUDENT BASIC DETAILS
        // =========================================================

        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? LastName { get; set; }

        [Required]
        [MaxLength(20)]
        public string Gender { get; set; } = string.Empty;

        [Required]
        public DateTime DateOfBirth { get; set; }

        [MaxLength(10)]
        public string? BloodGroup { get; set; }

        [MaxLength(500)]
        public string? StudentPhoto { get; set; }

        [EmailAddress]
        [MaxLength(150)]
        public string? StudentEmail { get; set; }

        [Phone]
        [MaxLength(20)]
        public string? StudentMobileNumber { get; set; }

        [MaxLength(20)]
        public string? AadhaarNumber { get; set; }

        [MaxLength(50)]
        public string? Nationality { get; set; }

        [MaxLength(50)]
        public string? Religion { get; set; }

        [MaxLength(50)]
        public string? Category { get; set; }


        // =========================================================
        // FATHER DETAILS
        // =========================================================

        [Required]
        [MaxLength(150)]
        public string FatherName { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? FatherOccupation { get; set; }

        [Phone]
        [MaxLength(20)]
        public string? FatherMobile { get; set; }

        [EmailAddress]
        [MaxLength(150)]
        public string? FatherEmail { get; set; }


        // =========================================================
        // MOTHER DETAILS
        // =========================================================

        [Required]
        [MaxLength(150)]
        public string MotherName { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? MotherOccupation { get; set; }

        [Phone]
        [MaxLength(20)]
        public string? MotherMobile { get; set; }

        [EmailAddress]
        [MaxLength(150)]
        public string? MotherEmail { get; set; }


        // =========================================================
        // GUARDIAN DETAILS
        // =========================================================

        [MaxLength(150)]
        public string? GuardianName { get; set; }

        [Phone]
        [MaxLength(20)]
        public string? GuardianMobile { get; set; }

        [EmailAddress]
        [MaxLength(150)]
        public string? GuardianEmail { get; set; }


        // =========================================================
        // INCOME
        // =========================================================

        public decimal? AnnualIncome { get; set; }


        // =========================================================
        // ADDRESS DETAILS
        // =========================================================

        [MaxLength(1000)]
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
        // ACADEMIC RELATIONSHIPS
        // =========================================================

        [Required]
        public int BoardId { get; set; }

        [ForeignKey(nameof(BoardId))]
        public Board? Board { get; set; }


        [Required]
        public int AcademicYearId { get; set; }

        [ForeignKey(nameof(AcademicYearId))]
        public AcademicYear? AcademicYear { get; set; }


        [Required]
        public int AcademicLevelId { get; set; }

        [ForeignKey(nameof(AcademicLevelId))]
        public AcademicLevel? AcademicLevelNavigation { get; set; }


        [Required]
        public int GroupId { get; set; }

        [ForeignKey(nameof(GroupId))]
        public Group? Group { get; set; }


        [Required]
        public int SectionId { get; set; }

        [ForeignKey(nameof(SectionId))]
        public Section? Section { get; set; }


        // =========================================================
        // PREVIOUS EDUCATION
        // =========================================================

        [MaxLength(200)]
        public string? PreviousSchool { get; set; }

        [MaxLength(100)]
        public string? PreviousBoard { get; set; }

        public decimal? PreviousPercentage { get; set; }

        public int? PreviousYearOfPassing { get; set; }

        [MaxLength(500)]
        public string? MarksMemo { get; set; }


        // =========================================================
        // ADMISSION / COURSE DETAILS
        // =========================================================

        [MaxLength(50)]
        public string? SecondLanguage { get; set; }

        [MaxLength(50)]
        public string? AdmissionType { get; set; }

        [MaxLength(50)]
        public string? Medium { get; set; }

        [MaxLength(50)]
        public string? ScholarshipStatus { get; set; }


        // =========================================================
        // ROLL NUMBER
        // =========================================================

        // Generated after admission approval.
        [MaxLength(50)]
        public string? RollNo { get; set; }


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


        // =========================================================
        // STATUS
        // =========================================================

        [MaxLength(30)]
        public string Status { get; set; } = "Pending";

        public bool IsVerified { get; set; }

        public bool IsApproved { get; set; }

        public bool IsRejected { get; set; }

        public bool IsActive { get; set; } = true;


        // =========================================================
        // OTHER
        // =========================================================

        [MaxLength(1000)]
        public string? Remarks { get; set; }


        // =========================================================
        // AUDIT
        // =========================================================

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }


    }
}
