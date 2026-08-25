using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CollegeManagement.API.Models
{
    [Table("Sections")]
    public class Section    
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SectionId { get; set; }

        public int? BoardId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Board { get; set; } = string.Empty;

        [Required]
        public int AcademicYearId { get; set; }

        public int? GroupId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Group { get; set; } = string.Empty;

        [MaxLength(100)]
        public string Programme { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string AcademicLevel { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string SectionName { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? RoomNumber { get; set; }

        public int? RoomId { get; set; }

        [Column("InchargeId")]
        public int? InchargeId { get; set; }

        [NotMapped]
        public int? ClassTeacherId
        {
            get => InchargeId;
            set => InchargeId = value;
        }

        [NotMapped]
        public int? FacultyId
        {
            get => InchargeId;
            set => InchargeId = value;
        }

        public int MaximumStrength { get; set; }

        [ForeignKey("InchargeId")]
        public CollegeManagement.API.Models.Faculty.Faculty? InchargeNavigation { get; set; }

        [ForeignKey("BoardId")]
        public Board? BoardNavigation { get; set; }

        [ForeignKey("AcademicYearId")]
        public AcademicYear? AcademicYear { get; set; }

        [ForeignKey("GroupId")]
        public Group? GroupNavigation { get; set; }

        [ForeignKey("RoomId")]
        public CollegeManagement.API.Models.Timetable.Room? RoomNavigation { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }
    }
}