using System.ComponentModel.DataAnnotations;

namespace CollegeManagement.API.DTOs.Fee
{
    public class CreateFeeStructureItemDto
    {
        [Required]
        public int FeeTypeId { get; set; }

        [Range(0.01, 999999999.99)]
        public decimal Amount { get; set; }

        public DateTime? DueDate { get; set; }

        public bool IsMandatory { get; set; }
    }
}