namespace CollegeManagement.API.DTOs.StudentAdmission
{
    public class CreateStudentAdmissionRequest
    {
        public string AdmissionNo { get; set; } = string.Empty;

        public DateTime AdmissionDate { get; set; }

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string? BloodGroup { get; set; }

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
        public int AcademicYearId { get; set; }
        public string AcademicLevel { get; set; } = string.Empty;
        public int GroupId { get; set; }
        public int SectionId { get; set; }

        public string? PreviousSchool { get; set; }
        public string? PreviousBoard { get; set; }
        public decimal? PreviousPercentage { get; set; }

        public IFormFile? StudentPhoto { get; set; }
        public IFormFile? BirthCertificate { get; set; }
        public IFormFile? TransferCertificate { get; set; }
        public IFormFile? StudyCertificate { get; set; }
        public IFormFile? AadhaarDocument { get; set; }
        public IFormFile? CommunityCertificate { get; set; }
        public IFormFile? IncomeCertificate { get; set; }
        public IFormFile? PassportPhoto { get; set; }
    }
}