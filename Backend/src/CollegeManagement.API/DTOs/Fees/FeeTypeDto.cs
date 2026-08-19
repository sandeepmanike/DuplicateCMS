namespace CollegeManagement.API.DTOs.Fee
{
    public class FeeTypeDto
    {
        public int FeeTypeId { get; set; }

        public string FeeTypeCode { get; set; } = null!;

        public string FeeTypeName { get; set; } = null!;

        public string? Description { get; set; }

        public bool IsMandatory { get; set; }

        public bool IsActive { get; set; }
    }
}