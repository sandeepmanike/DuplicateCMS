using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CollegeManagement.API.Models
{
    [Table("StudentAdmissions")]
    public class StudentAdmission
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AdmissionId { get; set; }

        [Required]
        [MaxLength(30)]
        public string AdmissionNo { get; set; } = string.Empty;

        [Required]
        public DateTime AdmissionDate { get; set; }

        // Student Details
        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string Gender { get; set; } = string.Empty;

        [Required]
        public DateTime DateOfBirth { get; set; }

        [MaxLength(10)]
        public string? BloodGroup { get; set; }

        [MaxLength(500)]
        public string? StudentPhoto { get; set; }

        [Required]
        [MaxLength(20)]
        public string AadhaarNumber { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? Nationality { get; set; }

        [MaxLength(100)]
        public string? Religion { get; set; }

        [MaxLength(100)]
        public string? Category { get; set; }

        // Parent Details
        [Required]
        [MaxLength(150)]
        public string FatherName { get; set; } = string.Empty;

        [Required]
        [MaxLength(150)]
        public string MotherName { get; set; } = string.Empty;

        [MaxLength(150)]
        public string? GuardianName { get; set; }

        [Required]
        [MaxLength(15)]
        public string ParentMobile { get; set; } = string.Empty;

        [MaxLength(150)]
        public string? ParentEmail { get; set; }

        [MaxLength(100)]
        public string? Occupation { get; set; }

        public decimal? AnnualIncome { get; set; }

        // Address
        [MaxLength(500)]
        public string? Address { get; set; }

        [MaxLength(100)]
        public string? City { get; set; }

        [MaxLength(100)]
        public string? District { get; set; }

        [MaxLength(100)]
        public string? State { get; set; }

        [MaxLength(10)]
        public string? Pincode { get; set; }

        // Academic Details
        [Required]
        public int BoardId { get; set; }

        [Required]
        public int AcademicYearId { get; set; }

        [Required]
        [MaxLength(50)]
        public string AcademicLevel { get; set; } = string.Empty;

        [Required]
        public int GroupId { get; set; }

        [Required]
        public int SectionId { get; set; }

        // Previous School
        [MaxLength(200)]
        public string? PreviousSchool { get; set; }

        [MaxLength(100)]
        public string? PreviousBoard { get; set; }

        public decimal? PreviousPercentage { get; set; }

        // Documents
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
        public string? PassportPhoto { get; set; }

        // Status
        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = "Pending";

        public bool IsVerified { get; set; } = false;

        public bool IsApproved { get; set; } = false;

        public bool IsRejected { get; set; } = false;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Navigation Properties
        [ForeignKey(nameof(BoardId))]
        public Board? Board { get; set; }

        [ForeignKey(nameof(AcademicYearId))]
        public AcademicYear? AcademicYear { get; set; }

        [ForeignKey(nameof(GroupId))]
        public Group? Group { get; set; }

        // Section navigation TEMPORARILY REMOVED
        // We'll add it after Section module is refactored to use proper foreign keys.
    }
}