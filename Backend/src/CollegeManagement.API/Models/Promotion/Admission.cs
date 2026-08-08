using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CollegeManagement.API.Models

{

    [Table("Admissions")]

    public class Admission
    {

        [Key]

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public long AdmissionId { get; set; }


        // Student Details

        [Required]

        [MaxLength(50)]

        public string AdmissionNo { get; set; } = string.Empty;


        [Required]

        public DateTime AdmissionDate { get; set; }


        [MaxLength(500)]

        public string? StudentPhoto { get; set; }


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

        public DateTime DOB { get; set; }


        [Required]

        [MaxLength(12)]

        public string Aadhaar { get; set; } = string.Empty;


        [MaxLength(10)]

        public string? BloodGroup { get; set; }


        [MaxLength(50)]

        public string? Nationality { get; set; }


        [MaxLength(50)]

        public string? Religion { get; set; }


        [MaxLength(100)]

        public string? Caste { get; set; }


        [MaxLength(50)]

        public string? Category { get; set; }


        // Parent Details

        [Required]

        [MaxLength(100)]

        public string FatherName { get; set; } = string.Empty;


        [Required]

        [MaxLength(100)]

        public string MotherName { get; set; } = string.Empty;


        [MaxLength(100)]

        public string? Guardian { get; set; }


        [Required]

        [MaxLength(15)]

        public string ParentMobile { get; set; } = string.Empty;


        [MaxLength(150)]

        public string? ParentEmail { get; set; }


        [MaxLength(100)]

        public string? Occupation { get; set; }


        [Column(TypeName = "decimal(12,2)")]

        public decimal? AnnualIncome { get; set; }


        // Address Details

        [Required]

        [MaxLength(500)]

        public string Address { get; set; } = string.Empty;


        [Required]

        [MaxLength(100)]

        public string City { get; set; } = string.Empty;


        [Required]

        [MaxLength(100)]

        public string District { get; set; } = string.Empty;


        [Required]

        [MaxLength(100)]

        public string State { get; set; } = string.Empty;


        [Required]

        [MaxLength(10)]

        public string Pincode { get; set; } = string.Empty;


        // Academic Details

        [Required]

        [MaxLength(100)]

        public string Board { get; set; } = string.Empty;


        [Required]

        [MaxLength(50)]

        public string AcademicYear { get; set; } = string.Empty;


        [Required]

        [MaxLength(100)]

        public string AcademicLevel { get; set; } = string.Empty;


        [Required]

        [MaxLength(100)]

        public string Group { get; set; } = string.Empty;


        [Required]

        [MaxLength(50)]

        public string Section { get; set; } = string.Empty;


        // Previous Education Details

        [MaxLength(200)]

        public string? PreviousSchool { get; set; }


        [MaxLength(100)]

        public string? PreviousBoard { get; set; }


        [Column(TypeName = "decimal(5,2)")]

        public decimal? PreviousPercentage { get; set; }


        // Workflow Details

        [MaxLength(30)]

        public string Status { get; set; } = "Draft";


        [MaxLength(500)]

        public string? VerificationRemarks { get; set; }


        [MaxLength(500)]

        public string? RejectionReason { get; set; }


        // Documentspublic ICollection<AdmissionDocument> Documents { get; set; }

           // = new List<AdmissionDocument>();

    }

}