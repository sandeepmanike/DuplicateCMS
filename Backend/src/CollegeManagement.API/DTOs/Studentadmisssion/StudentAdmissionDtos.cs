using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace CollegeManagement.API.DTOs.StudentAdmission
{
    // =========================================================
    // CREATE ADMISSION
    // =========================================================

    public class CreateStudentAdmissionRequest
    {
        // Admission
        [Required]
        public DateTime AdmissionDate { get; set; }
        public string? AdmissionNo { get; set; }

        

        public string? AdmissionType { get; set; }

        public string? AdmissionQuota { get; set; }


        // Academic Relations
        [Required]
        public int BoardId { get; set; }

        [Required]
        public int AcademicYearId { get; set; }

        [Required]
        public int AcademicLevelId { get; set; }

        [Required]
        public int GroupId { get; set; }

        [Required]
        public int ProgramId { get; set; }

        public string? Medium { get; set; }

        public string? SecondLanguage { get; set; }


        // Student
        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? LastName { get; set; }

        [Required]
        public string Gender { get; set; } = string.Empty;

        [Required]
        public DateTime DateOfBirth { get; set; }

        public string? BloodGroup { get; set; }

        [EmailAddress]
        public string? StudentEmail { get; set; }

        public string? StudentMobileNumber { get; set; }


        // Photo
        [Required]
        public IFormFile StudentPhoto { get; set; } = null!;


        // Personal
        public string? AadhaarNumber { get; set; }

        public string? Nationality { get; set; }

        public string? Religion { get; set; }

        public string? Category { get; set; }


        // Father - Optional
        public string? FatherName { get; set; }

        public string? FatherOccupation { get; set; }

        public string? FatherMobile { get; set; }

        [EmailAddress]
        public string? FatherEmail { get; set; }


        // Mother - Optional
        public string? MotherName { get; set; }

        public string? MotherOccupation { get; set; }

        public string? MotherMobile { get; set; }

        [EmailAddress]
        public string? MotherEmail { get; set; }


        // Guardian - Optional
        public string? GuardianName { get; set; }

        public string? GuardianMobile { get; set; }

        [EmailAddress]
        public string? GuardianEmail { get; set; }


        // Other
        public decimal? AnnualIncome { get; set; }

        public string? ScholarshipStatus { get; set; }


        // Address
        public string? Address { get; set; }

        public string? City { get; set; }

        public string? District { get; set; }

        public string? State { get; set; }

        public string? Pincode { get; set; }


        // Previous Education
        public string? PreviousSchool { get; set; }

        public string? PreviousBoard { get; set; }

        public decimal? PreviousPercentage { get; set; }

        public int? PreviousYearOfPassing { get; set; }
    }


    // =========================================================
    // UPDATE ADMISSION
    // =========================================================

    public class UpdateStudentAdmissionRequest
    {
        public DateTime AdmissionDate { get; set; }
        public string? AdmissionNo { get; set; }

       

        public string? AdmissionType { get; set; }

        public string? AdmissionQuota { get; set; }


        // Academic
        [Required]
        public int BoardId { get; set; }

        [Required]
        public int AcademicYearId { get; set; }

        [Required]
        public int AcademicLevelId { get; set; }

        [Required]
        public int GroupId { get; set; }

        [Required]
        public int ProgramId { get; set; }

        public string? Medium { get; set; }

        public string? SecondLanguage { get; set; }


        // Student
        [Required]
        public string FirstName { get; set; } = string.Empty;

        public string? LastName { get; set; }

        [Required]
        public string Gender { get; set; } = string.Empty;

        [Required]
        public DateTime DateOfBirth { get; set; }

        public string? BloodGroup { get; set; }

        [EmailAddress]
        public string? StudentEmail { get; set; }

        public string? StudentMobileNumber { get; set; }


        // Photo - optional on update
        public IFormFile? StudentPhoto { get; set; }


        // Personal
        public string? AadhaarNumber { get; set; }

        public string? Nationality { get; set; }

        public string? Religion { get; set; }

        public string? Category { get; set; }


        // Father - Optional
        public string? FatherName { get; set; }

        public string? FatherOccupation { get; set; }

        public string? FatherMobile { get; set; }

        [EmailAddress]
        public string? FatherEmail { get; set; }


        // Mother - Optional
        public string? MotherName { get; set; }

        public string? MotherOccupation { get; set; }

        public string? MotherMobile { get; set; }

        [EmailAddress]
        public string? MotherEmail { get; set; }


        // Guardian - Optional
        public string? GuardianName { get; set; }

        public string? GuardianMobile { get; set; }

        [EmailAddress]
        public string? GuardianEmail { get; set; }


        // Other
        public decimal? AnnualIncome { get; set; }

        public string? ScholarshipStatus { get; set; }


        // Address
        public string? Address { get; set; }

        public string? City { get; set; }

        public string? District { get; set; }

        public string? State { get; set; }

        public string? Pincode { get; set; }


        // Previous Education
        public string? PreviousSchool { get; set; }

        public string? PreviousBoard { get; set; }

        public decimal? PreviousPercentage { get; set; }

        public int? PreviousYearOfPassing { get; set; }
    }


    // =========================================================
    // RESPONSE
    // =========================================================

    public class StudentAdmissionResponseDto
    {
        public int AdmissionId { get; set; }

        public string? AdmissionNo { get; set; }

        public DateTime AdmissionDate { get; set; }

        public string? AdmissionType { get; set; }

        public string? AdmissionQuota { get; set; }


        // Academic Relations

        public int BoardId { get; set; }

        public string? BoardName { get; set; }

        public int AcademicYearId { get; set; }

        public string? AcademicYearName { get; set; }

        public int AcademicLevelId { get; set; }

        public string? AcademicLevelName { get; set; }

        public int GroupId { get; set; }

        public string? GroupName { get; set; }

        public int ProgramId { get; set; }

        public string? ProgramName { get; set; }


        // Student

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? Gender { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public string? BloodGroup { get; set; }

        public string? StudentEmail { get; set; }

        public string? StudentMobileNumber { get; set; }

        public string? StudentPhoto { get; set; }


        // Personal

        public string? AadhaarNumber { get; set; }

        public string? Nationality { get; set; }

        public string? Religion { get; set; }

        public string? Category { get; set; }


        // Father

        public string? FatherName { get; set; }

        public string? FatherOccupation { get; set; }

        public string? FatherMobile { get; set; }

        public string? FatherEmail { get; set; }


        // Mother

        public string? MotherName { get; set; }

        public string? MotherOccupation { get; set; }

        public string? MotherMobile { get; set; }

        public string? MotherEmail { get; set; }


        // Guardian

        public string? GuardianName { get; set; }

        public string? GuardianMobile { get; set; }

        public string? GuardianEmail { get; set; }


        // Other

        public decimal? AnnualIncome { get; set; }

        public string? ScholarshipStatus { get; set; }

        public string? Medium { get; set; }

        public string? SecondLanguage { get; set; }


        // Address

        public string? Address { get; set; }

        public string? City { get; set; }

        public string? District { get; set; }

        public string? State { get; set; }

        public string? Pincode { get; set; }


        // Previous Education

        public string? PreviousSchool { get; set; }

        public string? PreviousBoard { get; set; }

        public decimal? PreviousPercentage { get; set; }

        public int? PreviousYearOfPassing { get; set; }


        // Admission Status

        public string? Status { get; set; }

        public bool IsVerified { get; set; }

        public bool IsApproved { get; set; }

        public bool IsRejected { get; set; }

        public string? RejectionReason { get; set; }

        public string? Remarks { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }


        // =====================================================
        // ALLOCATION - populated only after approval
        // =====================================================

        public int? SectionId { get; set; }

        public string? SectionName { get; set; }

        public string? RollNo { get; set; }
    }


    // =========================================================
    // VERIFY
    // =========================================================

    public class VerifyStudentAdmissionRequest
    {
        [Required]
        public int AdmissionId { get; set; }

        public string? Remarks { get; set; }
    }


    // =========================================================
    // APPROVE
    // =========================================================

    public class ApproveStudentAdmissionRequest
    {
        [Required]
        public int AdmissionId { get; set; }

        public string? Remarks { get; set; }
    }


    // =========================================================
    // REJECT
    // =========================================================

    public class RejectStudentAdmissionRequest
    {
        [Required]
        public int AdmissionId { get; set; }

        [Required]
        public string RejectionReason { get; set; } = string.Empty;

        public string? Remarks { get; set; }
    }


    // =========================================================
    // SECTION ALLOCATION
    // =========================================================

    public class AllocateSectionRequest
    {
        [Required]
        public int AdmissionId { get; set; }

        [Required]
        public int SectionId { get; set; }
    }


    // =========================================================
    // BULK SECTION ALLOCATION
    // =========================================================

    public class BulkSectionAllocationRequest
    {
        [Required]
        public int SectionId { get; set; }

        [Required]
        public List<int> AdmissionIds { get; set; } = new();
    }


    // =========================================================
    // BULK ROLL NUMBER ALLOCATION
    // =========================================================

    public class BulkRollNumberAllocationRequest
    {
        [Required]
        public int SectionId { get; set; }

        [Required]
        public int StartingRollNumber { get; set; }

        [Required]
        public List<int> AdmissionIds { get; set; } = new();
    }
}