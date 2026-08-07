using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CollegeManagement.API.Models

{

    [Table("Students")]

    public class Student
    {

        [Key]

        public int StudentId { get; set; }


        // ==========================// Admission Details// ==========================

        [Required]

        [MaxLength(30)]

        public string AdmissionNo { get; set; } = string.Empty;


        [Required]

        [MaxLength(30)]

        public string RollNo { get; set; } = string.Empty;


        [Required]

        [MaxLength(150)]

        public string StudentName { get; set; } = string.Empty;


        [MaxLength(500)]

        public string? Photo { get; set; }


        // ==========================// Personal Information// ==========================

        [Required]

        [MaxLength(20)]

        public string Gender { get; set; } = string.Empty;


        [Required]

        public DateOnly DateOfBirth { get; set; }


        [MaxLength(10)]

        public string? BloodGroup { get; set; }


        [Required]

        [MaxLength(150)]

        [EmailAddress]

        public string Email { get; set; } = string.Empty;


        [Required]

        [MaxLength(20)]

        public string MobileNumber { get; set; } = string.Empty;


        [MaxLength(20)]

        public string? AadhaarNumber { get; set; }


        [MaxLength(500)]

        public string? Address { get; set; }


        // ==========================// Academic Information// ==========================

        [Required]

        [MaxLength(100)]

        public string Board { get; set; } = string.Empty;


        [Required]

        public int AcademicYearId { get; set; }


        [Required]

        [MaxLength(50)]

        public string AcademicLevel { get; set; } = string.Empty;


        [Required]

        public int GroupId { get; set; }


        [Required]

        [MaxLength(20)]

        public string Section { get; set; } = string.Empty;


        [Required]

        public DateOnly AdmissionDate { get; set; }


        [MaxLength(50)]

        public string? AdmissionType { get; set; }


        [MaxLength(50)]

        public string? Medium { get; set; }


        [MaxLength(200)]

        public string? PreviousSchool { get; set; }


        [MaxLength(50)]

        public string? PreviousHallTicketNumber { get; set; }


        [MaxLength(50)]

        public string? StudentCategory { get; set; }


        [MaxLength(50)]

        public string? ScholarshipStatus { get; set; }


        // ==========================// Parent Details// ==========================

        [MaxLength(150)]

        public string? FatherName { get; set; }


        [MaxLength(20)]

        public string? FatherMobile { get; set; }


        [MaxLength(150)]

        public string? MotherName { get; set; }


        [MaxLength(20)]

        public string? MotherMobile { get; set; }


        [MaxLength(150)]

        public string? GuardianName { get; set; }


        [MaxLength(20)]

        public string? GuardianMobile { get; set; }


        // ==========================// Fee Information// ==========================

        [Column(TypeName = "decimal(10,2)")]

        public decimal FeeAmount { get; set; }


        [Column(TypeName = "decimal(10,2)")]

        public decimal FeePaid { get; set; }


        [Column(TypeName = "decimal(10,2)")]

        public decimal ScholarshipAmount { get; set; }


        [MaxLength(30)]

        public string? FeeStatus { get; set; }


        // ==========================// Attendance & Performance// ==========================

        [Column(TypeName = "decimal(5,2)")]

        public decimal AttendancePercentage { get; set; }


        [MaxLength(20)]

        public string? PerformanceGrade { get; set; }


        [Column(TypeName = "decimal(5,2)")]

        public decimal? CGPA { get; set; }


        public int? Rank { get; set; }


        [MaxLength(500)]

        public string? Remarks { get; set; }


        // ==========================// Login Information// ==========================

        [Required]

        [MaxLength(255)]

        public string PasswordHash { get; set; } = string.Empty;


        public bool IsFirstLogin { get; set; } = true;


        public DateTime? LastLogin { get; set; }


        // ==========================// Status// ==========================public bool IsActive { get; set; } = true;


        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;


        public DateTime? UpdatedAt { get; set; }

    }

}