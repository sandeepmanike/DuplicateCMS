using System;using System.ComponentModel.DataAnnotations;using System.ComponentModel.DataAnnotations.Schema;
namespace CollegeManagement.API.Models
{
    [Table("Sections")]
    public class Section    
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SectionId { get; set; }
        [Required]
        [MaxLength(100)]
        public string Board { get; set; } = string.Empty;
        [Required]
        public int AcademicYearId { get; set; }
        [Required]
        [MaxLength(100)]
        public string Group { get; set; } = string.Empty;
        [Required]
        [MaxLength(50)]
        public string AcademicLevel { get; set; } = string.Empty;
        [Required]
        [MaxLength(50)]
        public string SectionName { get; set; } = string.Empty;
        [MaxLength(50)]
        public string? RoomNumber { get; set; }
        public int? ClassTeacherId { get; set; }
        public int MaximumStrength { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
 