using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CollegeManagement.API.Models
{
    public class Examination
    {
        [Key]
        public int ExaminationId { get; set; }

        [Required]
        [StringLength(150)]
        public string ExamName { get; set; } = string.Empty;

        [Required]
        public int BoardId { get; set; }

        [Required]
        public int AcademicYearId { get; set; }

        [Required]
        public int AcademicLevelId { get; set; }

        [Required]
        public int GroupId { get; set; }

        [Required]
        public int AssessmentTypeId { get; set; }

        [Required]
        public DateOnly StartDate { get; set; }

        [Required]
        public DateOnly EndDate { get; set; }

        public bool IsActive { get; set; } = true;

        // Navigation Properties
        public Board? Board { get; set; }
        public AcademicYear? AcademicYear { get; set; }
        public AcademicLevel? AcademicLevel { get; set; }
        public Group? Group { get; set; }
        public AssessmentType? AssessmentType { get; set; }

        public string Status { get; set; } = "ACTIVE";
    }
}