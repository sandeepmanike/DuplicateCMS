namespace CollegeManagement.API.DTOs.Fee
{
    public class AssignFeeDto
    {
        public int StudentId { get; set; }
        public int FeeStructureId { get; set; }
        public string PaymentMode { get; set; } = string.Empty;
    }
}