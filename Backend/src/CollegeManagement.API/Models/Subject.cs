using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CollegeManagement.API.Models
{
    [Table("Subjects")]
    public class Subject
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SubjectId { get; set; }

        public int BoardId { get; set; }

        public int GroupId { get; set; }

        public int AcademicLevelId { get; set; }

        [NotMapped]
        public string? BoardName { get; set; }

        [NotMapped]
        public string? GroupName { get; set; }

        [NotMapped]
        public string? AcademicLevelName { get; set; }

        [Required, MaxLength(150)]
        public string SubjectName { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        public string SubjectCode { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        public string SubjectType { get; set; } = string.Empty;

        public bool Theory { get; set; }
        public bool Practical { get; set; }
        public bool Language { get; set; }
        public bool Elective { get; set; }
        public int InternalMarks { get; set; }
        public int PracticalMarks { get; set; }
        public int ExternalMarks { get; set; }
        public int TotalMarks { get; set; }
        public int PassingMarks { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        [ForeignKey(nameof(BoardId))]
        public virtual Board? BoardNavigation { get; set; }

        [ForeignKey(nameof(GroupId))]
        public virtual Group? GroupNavigation { get; set; }

        [ForeignKey(nameof(AcademicLevelId))]
        public virtual AcademicLevel? AcademicLevelNavigation { get; set; }
    }
}
