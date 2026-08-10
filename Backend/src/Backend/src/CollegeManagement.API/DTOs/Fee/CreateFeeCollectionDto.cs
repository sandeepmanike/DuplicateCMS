public class CreateFeeCollectionDto
{
    public int StudentId { get; set; }
    public int FeeStructureId { get; set; }
    public decimal Amount { get; set; }

    public string? PaymentMode { get; set; }
    public decimal Discount { get; set; }
    public decimal Fine { get; set; }
    public string? TransactionNumber { get; set; }
}