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

        public int? BoardId { get; set; }

        public int? AcademicYearId { get; set; }

        public int? GroupId { get; set; }

        [MaxLength(100)]
        public string Board { get; set; } = string.Empty;

        [MaxLength(100)]
        public string Group { get; set; } = string.Empty;

        [MaxLength(100)]
        public string AcademicLevel { get; set; } = string.Empty;

        [NotMapped]
        private int? _academicLevelId;

        [NotMapped]
        public int AcademicLevelId
        {
            get
            {
                if (_academicLevelId.HasValue && _academicLevelId.Value > 0)
                    return _academicLevelId.Value;

                if (!string.IsNullOrEmpty(AcademicLevel))
                {
                    if (int.TryParse(AcademicLevel.Trim(), out var parsedId))
                        return parsedId;

                    if (AcademicLevel.Contains("2") || AcademicLevel.ToLower().Contains("2nd") || AcademicLevel.ToLower().Contains("second"))
                        return 2;

                    return 1;
                }
                return 1;
            }
            set => _academicLevelId = value;
        }

        [NotMapped]
        public string? BoardName
        {
            get => Board;
            set => Board = value ?? string.Empty;
        }

        [NotMapped]
        public string? AcademicYearName { get; set; }

        [NotMapped]
        public string? AcademicLevelName
        {
            get => AcademicLevel;
            set => AcademicLevel = value ?? string.Empty;
        }

        [NotMapped]
        public string? GroupName
        {
            get => Group;
            set => Group = value ?? string.Empty;
        }

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
        public Board? BoardNavigation { get; set; }

        [ForeignKey(nameof(AcademicYearId))]
        public AcademicYear? AcademicYear { get; set; }

        [ForeignKey(nameof(GroupId))]
        public Group? GroupNavigation { get; set; }
    }
}