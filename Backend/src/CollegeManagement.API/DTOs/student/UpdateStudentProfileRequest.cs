using System.ComponentModel.DataAnnotations;

namespace CollegeManagement.API.DTOs.Students
{
    public class UpdateStudentProfileRequest
    {
        [Required]
        [MaxLength(150)]
        public string StudentName { get; set; } = string.Empty;

        public string? Photo { get; set; }

        [Required]
        [MaxLength(20)]
        public string Gender { get; set; } = string.Empty;

        [Required]
        public DateOnly DateOfBirth { get; set; }

        [MaxLength(10)]
        public string? BloodGroup { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [RegularExpression(
            @"^[6-9][0-9]{9}$",
            ErrorMessage =
                "Mobile number must be exactly 10 digits and start with 6-9.")]
        public string MobileNumber { get; set; } = string.Empty;

        [RegularExpression(
            @"^[0-9]{12}$",
            ErrorMessage =
                "Aadhaar number must be exactly 12 digits.")]
        public string? AadhaarNumber { get; set; }


        // =========================================================
        // ADDRESS
        // =========================================================

        public string? Address { get; set; }
        public string? City { get; set; }

        public string? District { get; set; }

        public string? State { get; set; }

        [RegularExpression(
            @"^[1-9][0-9]{5}$",
            ErrorMessage =
                "Pincode must be exactly 6 digits.")]
        public string? Pincode { get; set; }


        // =========================================================
        // OTHER PERSONAL DETAILS
        // =========================================================

        public string? Nationality { get; set; }

        public string? Religion { get; set; }

        public string? Category { get; set; }


        // =========================================================
        // FATHER
        // =========================================================

        public string? FatherName { get; set; }

        public string? FatherOccupation { get; set; }

        [RegularExpression(
            @"^[6-9][0-9]{9}$",
            ErrorMessage =
                "Father mobile must be a valid 10-digit mobile number.")]
        public string? FatherMobile { get; set; }

        [EmailAddress]
        public string? FatherEmail { get; set; }


        // =========================================================
        // MOTHER
        // =========================================================

        public string? MotherName { get; set; }

        public string? MotherOccupation { get; set; }

        [RegularExpression(
            @"^[6-9][0-9]{9}$",
            ErrorMessage =
                "Mother mobile must be a valid 10-digit mobile number.")]
        public string? MotherMobile { get; set; }

        [EmailAddress]
        public string? MotherEmail { get; set; }


        // =========================================================
        // GUARDIAN
        // =========================================================

        public string? GuardianName { get; set; }

        [RegularExpression(
            @"^[6-9][0-9]{9}$",
            ErrorMessage =
                "Guardian mobile must be a valid 10-digit mobile number.")]
        public string? GuardianMobile { get; set; }

        [EmailAddress]
        public string? GuardianEmail { get; set; }


        // =========================================================
        // REMARKS
        // =========================================================

        public string? Remarks { get; set; }
    }
}