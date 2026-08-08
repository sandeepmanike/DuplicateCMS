public class FeePaymentHistoryDto
{
    public string ReceiptId { get; set; } = string.Empty;
    public string FeeType { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime PaymentDate { get; set; }
    public string AcademicYear { get; set; } = string.Empty;
}