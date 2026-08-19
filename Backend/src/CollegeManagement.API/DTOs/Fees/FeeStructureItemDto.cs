namespace CollegeManagement.API.DTOs.Fee
{
    public class FeeStructureItemDto
    {
        public int FeeStructureItemId { get; set; }

        public int FeeTypeId { get; set; }

        public string FeeTypeCode { get; set; } = null!;

        public string FeeTypeName { get; set; } = null!;

        public decimal Amount { get; set; }

        public DateTime? DueDate { get; set; }

        public bool IsMandatory { get; set; }

        public bool IsActive { get; set; }
    }
}