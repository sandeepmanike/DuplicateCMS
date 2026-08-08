using System;
using System.ComponentModel.DataAnnotations;

namespace CollegeManagement.API.Models
{
    public class ExamSchedule
    {
        [Key]
        public int ExamScheduleId { get; set; }

        [Required]
        public int ExaminationId { get; set; }

        [Required]
        public int SubjectId { get; set; }

        [Required]
        public DateOnly ExamDate { get; set; }

        [Required]
        public TimeOnly ExamTime { get; set; }

        [Required]
        [StringLength(100)]
        public string Hall { get; set; } = string.Empty;

        [Required]
        [StringLength(150)]
        public string Invigilator { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        // Navigation Properties
        public Examination? Examination { get; set; }
        public Subject? Subject { get; set; }
    }
}