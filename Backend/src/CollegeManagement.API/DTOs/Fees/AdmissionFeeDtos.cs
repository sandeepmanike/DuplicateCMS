// =====================================================
// ADMISSION FEE DTOs
// =====================================================

public class AdmissionFeeAssignDto
{
    public int AdmissionId { get; set; }

    public int FeeStructureId { get; set; }

    public List<AdmissionFeeItemDto> FeeItems { get; set; } = new();
}

public class AdmissionFeeItemDto
{
    public int FeeTypeId { get; set; }

    public decimal Amount { get; set; }
}


public class AdmissionFeePaymentDto
{
    public decimal Amount { get; set; }

    public string PaymentMode { get; set; } = string.Empty;

    public string? TransactionNumber { get; set; }

    public DateTime PaymentDate { get; set; }

    public string? Remarks { get; set; }
}


public class AdmissionFeeSummaryDto
{
    public int AdmissionId { get; set; }

    public int FeeStructureId { get; set; }

    public decimal TotalFee { get; set; }

    public decimal DiscountAmount { get; set; }

    public decimal FinalPayable { get; set; }

    public decimal AdmissionFeeAmount { get; set; }

    public decimal AdmissionFeePaid { get; set; }

    public decimal RemainingAdmissionFee { get; set; }

    public decimal TotalPaid { get; set; }

    public decimal TotalDue { get; set; }
}