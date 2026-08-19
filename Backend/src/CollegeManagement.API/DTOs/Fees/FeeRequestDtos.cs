namespace CollegeManagement.API.DTOs.Fee
{
    public class FeeStructureRequestDto
    {
        public int BoardId { get; set; }
        public int AcademicYearId { get; set; }
        public int AcademicLevelId { get; set; }
        public int GroupId { get; set; }
        public int FeeTypeId { get; set; }
        public decimal Amount { get; set; }
        public DateTime? DueDate { get; set; }
    }

    public class StudentFeeAssignmentRequestDto
    {
        public int StudentId { get; set; }
        public int StudentAdmissionId { get; set; }
        public int FeeStructureId { get; set; }
    }

    public class StudentFeeAssignmentUpdateDto
    {
        public decimal DiscountAmount { get; set; }
        public decimal ScholarshipAmount { get; set; }
    }

    public class FeePaymentRequestDto
    {
        public int StudentFeeAssignmentId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMode { get; set; } = string.Empty;
        public string? TransactionNumber { get; set; }
        public string? Remarks { get; set; }
    }

    public class DiscountRequestDto
    {
        public int AdmissionId { get; set; }

        public decimal DiscountAmount { get; set; }
        public string? Reason { get; set; }

    }

    public class ScholarshipRequestDto
    {
        public int StudentFeeAssignmentId { get; set; }
        public decimal ScholarshipAmount { get; set; }
        public string ScholarshipName { get; set; } = string.Empty;
    }

    public class FineRequestDto
    {
        public int StudentFeeAssignmentId { get; set; }
        public decimal FineAmount { get; set; }
        public string? Reason { get; set; }
    }

    public class RefundRequestDto
    {
        public int PaymentId { get; set; }
        public decimal RefundAmount { get; set; }
        public string? Reason { get; set; }
    }
}