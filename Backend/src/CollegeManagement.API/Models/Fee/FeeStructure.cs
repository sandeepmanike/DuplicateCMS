using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CollegeManagement.API.Models.Fee
{
    [Table("FeeStructures")]
    public class FeeStructure
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int FeeStructureId { get; set; }

        [Required]
        public int BoardId { get; set; }

        [Required]
        public int AcademicYearId { get; set; }

        [Required]
        public int AcademicLevelId { get; set; }

        [Required]
        public int GroupId { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Navigation properties

        [ForeignKey(nameof(BoardId))]
        public virtual Board Board { get; set; } = null!;

        [ForeignKey(nameof(AcademicYearId))]
        public virtual AcademicYear AcademicYear { get; set; } = null!;

        [ForeignKey(nameof(AcademicLevelId))]
        public virtual AcademicLevel AcademicLevel { get; set; } = null!;

        [ForeignKey(nameof(GroupId))]
        public virtual Group Group { get; set; } = null!;

        public virtual ICollection<FeeStructureItems> Items { get; set; }
            = new List<FeeStructureItems>();
    }
}