using System;

namespace CollegeManagement.API.DTOs.StudentAdmission
{
    public class StudentAdmissionResponseDto
    {
        // =========================================================
        // ADMISSION
        // =========================================================

        public int AdmissionId { get; set; }

        public int? StudentId { get; set; }

        public string AdmissionNo { get; set; } = string.Empty;

        public DateTime AdmissionDate { get; set; }

        public string? AdmissionQuota { get; set; }


        // =========================================================
        // STUDENT DETAILS
        // =========================================================

        public string FirstName { get; set; } = string.Empty;

        public string? LastName { get; set; }

        public string Gender { get; set; } = string.Empty;

        public DateTime DateOfBirth { get; set; }

        public string? BloodGroup { get; set; }

        public string? StudentPhoto { get; set; }

        public string? Email { get; set; }
        public string? MobileNumber { get; set; }

        public string? RollNo { get; set; }

        public string? AadhaarNumber { get; set; }

        public string? Nationality { get; set; }

        public string? Religion { get; set; }

        public string? Category { get; set; }


        // =========================================================
        // FATHER DETAILS
        // =========================================================

        public string? FatherName { get; set; }

        public string? FatherOccupation { get; set; }

        public string? FatherMobile { get; set; }

        public string? FatherEmail { get; set; }


        // =========================================================
        // MOTHER DETAILS
        // =========================================================

        public string? MotherName { get; set; }

        public string? MotherOccupation { get; set; }

        public string? MotherMobile { get; set; }

        public string? MotherEmail { get; set; }


        // =========================================================
        // GUARDIAN DETAILS
        // =========================================================

        public string? GuardianName { get; set; }

        public string? GuardianMobile { get; set; }

        public string? GuardianEmail { get; set; }


        // =========================================================
        // INCOME
        // =========================================================

        public decimal? AnnualIncome { get; set; }


        // =========================================================
        // ADDRESS
        // =========================================================

        public string? Address { get; set; }
        public string? City { get; set; }

        public string? District { get; set; }

        public string? State { get; set; }

        public string? Pincode { get; set; }


        // =========================================================
        // BOARD
        // =========================================================

        public int BoardId { get; set; }

        public string? BoardName { get; set; }


        // =========================================================
        // ACADEMIC YEAR
        // =========================================================

        public int AcademicYearId { get; set; }

        public string? AcademicYearName { get; set; }


        // =========================================================
        // ACADEMIC LEVEL
        // =========================================================

        public int AcademicLevelId { get; set; }

        public string? AcademicLevelName { get; set; }


        // =========================================================
        // GROUP
        // =========================================================

        public int GroupId { get; set; }

        public string? GroupName { get; set; }


        // =========================================================
        // SECTION
        // =========================================================

        public int SectionId { get; set; }

        public string? SectionName { get; set; }


        // =========================================================
        // ADMISSION ACADEMIC DETAILS
        // =========================================================

        public string? Medium { get; set; }

        public string? SecondLanguage { get; set; }

        public string? AdmissionType { get; set; }


        // =========================================================
        // PREVIOUS EDUCATION
        // =========================================================

        public string? PreviousSchool { get; set; }

        public int? PreviousYearOfPassing { get; set; }

        public string? PreviousBoard { get; set; }

        public decimal? PreviousPercentage { get; set; }


        // =========================================================
        // SCHOLARSHIP
        // =========================================================

        public string? ScholarshipStatus { get; set; }


        // =========================================================
        // DOCUMENTS
        // =========================================================

        public string? BirthCertificate { get; set; }

        public string? TransferCertificate { get; set; }

        public string? StudyCertificate { get; set; }

        public string? AadhaarDocument { get; set; }

        public string? CommunityCertificate { get; set; }

        public string? IncomeCertificate { get; set; }

        public string? CasteCertificate { get; set; }

        public string? TenthCertificate { get; set; }

        public string? MarksMemo { get; set; }


        // =========================================================
        // REMARKS
        // =========================================================

        public string? Remarks { get; set; }


        // =========================================================
        // STATUS
        // =========================================================

        public string Status { get; set; } = "Pending";

        public bool IsVerified { get; set; }

        public bool IsApproved { get; set; }

        public bool IsRejected { get; set; }

        public bool IsActive { get; set; }


        // =========================================================
        // AUDIT
        // =========================================================

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}