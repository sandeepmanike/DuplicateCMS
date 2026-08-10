namespace CollegeManagement.API.DTOs.StudentAdmission
{
    public class StudentAdmissionResponseDto
    {
        public int AdmissionId { get; set; }

        public string AdmissionNo { get; set; } = string.Empty;

        public DateTime AdmissionDate { get; set; }

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string Gender { get; set; } = string.Empty;

        public DateTime DateOfBirth { get; set; }

        public string? BloodGroup { get; set; }

        public string? StudentPhoto { get; set; }

        public string AadhaarNumber { get; set; } = string.Empty;

        public string? Nationality { get; set; }

        public string? Religion { get; set; }

        public string? Category { get; set; }

        public string FatherName { get; set; } = string.Empty;

        public string MotherName { get; set; } = string.Empty;

        public string? GuardianName { get; set; }

        public string ParentMobile { get; set; } = string.Empty;

        public string? ParentEmail { get; set; }

        public string? Occupation { get; set; }

        public decimal? AnnualIncome { get; set; }

        public string? Address { get; set; }

        public string? City { get; set; }

        public string? District { get; set; }

        public string? State { get; set; }

        public string? Pincode { get; set; }

        public int BoardId { get; set; }

        public string BoardName { get; set; } = string.Empty;

        public int AcademicYearId { get; set; }

        public string AcademicYearName { get; set; } = string.Empty;

        public string AcademicLevel { get; set; } = string.Empty;

        public int GroupId { get; set; }

        public string GroupName { get; set; } = string.Empty;

        public int SectionId { get; set; }

        public string SectionName { get; set; } = string.Empty;

        public string? PreviousSchool { get; set; }

        public string? PreviousBoard { get; set; }

        public decimal? PreviousPercentage { get; set; }

        public string Status { get; set; } = string.Empty;

        public bool IsVerified { get; set; }

        public bool IsApproved { get; set; }

        public bool IsRejected { get; set; }

        public bool IsActive { get; set; }
    }
}