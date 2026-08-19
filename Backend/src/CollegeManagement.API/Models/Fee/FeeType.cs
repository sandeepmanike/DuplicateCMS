using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CollegeManagement.API.Models.Fee
{
    [Table("FeeTypes")]
    public class FeeType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int FeeTypeId { get; set; }

        [Required]
        [MaxLength(30)]
        public string FeeTypeCode { get; set; } = null!;

        [Required]
        [MaxLength(100)]
        public string FeeTypeName { get; set; } = null!;

        [MaxLength(500)]
        public string? Description { get; set; }

        public bool IsMandatory { get; set; } = false;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }
    }
}