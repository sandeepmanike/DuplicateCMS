using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CollegeManagement.API.Models
{
    [Table("Subjects")]
    public class Subject
    {
        [Key]
        [Column("SubjectId")]
        public int SubjectId { get; set; }

        [NotMapped]
        public int Id
        {
            get => SubjectId;
            set => SubjectId = value;
        }

        public int BoardId { get; set; }

        public int GroupId { get; set; }

        public int AcademicLevelId { get; set; }

        public int? AcademicYearId { get; set; }

        [Required]
        [StringLength(150)]
        public string SubjectName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string SubjectCode { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string SubjectType { get; set; } = "Theory"; // Theory, Practical

        public bool Theory { get; set; } = true;

        public bool Practical { get; set; } = false;

        public bool Language { get; set; } = false;

        public bool Elective { get; set; } = false;

        public int InternalMarks { get; set; } = 0;

        public int PracticalMarks { get; set; } = 0;

        public int ExternalMarks { get; set; } = 0;

        public int TotalMarks { get; set; } = 100;

        public int PassingMarks { get; set; } = 35;

        public string AcademicLevel { get; set; } = string.Empty;

        public string Board { get; set; } = string.Empty;

        [NotMapped]
        public string BoardName => BoardNavigation?.BoardName ?? Board ?? string.Empty;

        [NotMapped]
        public string GroupName => GroupNavigation?.GroupName ?? string.Empty;

        [NotMapped]
        public string AcademicLevelName => AcademicLevelNavigation?.LevelName ?? AcademicLevel ?? string.Empty;

        [NotMapped]
        public int WeeklyPeriods { get; set; } = 4;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Navigation Properties
        [ForeignKey(nameof(BoardId))]
        public virtual Board? BoardNavigation { get; set; }

        [ForeignKey(nameof(AcademicLevelId))]
        public virtual AcademicLevel? AcademicLevelNavigation { get; set; }

        [ForeignKey(nameof(GroupId))]
        public virtual Group? GroupNavigation { get; set; }
    }
}
