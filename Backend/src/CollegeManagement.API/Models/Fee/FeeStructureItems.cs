using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CollegeManagement.API.Models.Fee
{
    [Table("FeeStructureItems")]
    public class FeeStructureItems
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int FeeStructureItemId { get; set; }

        [Required]
        public int FeeStructureId { get; set; }

        [Required]
        public int FeeTypeId { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Amount { get; set; }

        public DateTime? DueDate { get; set; }

        public bool IsMandatory { get; set; } = false;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Navigation properties

        [ForeignKey(nameof(FeeStructureId))]
        public virtual FeeStructure FeeStructure { get; set; } = null!;

        [ForeignKey(nameof(FeeTypeId))]
        public virtual FeeType FeeType { get; set; } = null!;
    }
}