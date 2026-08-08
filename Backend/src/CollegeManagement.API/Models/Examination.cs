using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CollegeManagement.API.Models
{
    [Table("Examinations")]
    public class Examination
    {
        [Key]
        [Column("ExamId")]
        public int ExaminationId { get; set; }

        [NotMapped]
        public int ExamId
        {
            get => ExaminationId;
            set => ExaminationId = value;
        }

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