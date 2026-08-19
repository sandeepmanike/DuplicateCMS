using CollegeManagement.API.DTOs.Fee;

namespace CollegeManagement.API.Services.Interfaces
{
    public interface IFeeService
    {
        Task<IEnumerable<dynamic>> GetFeeTypesAsync();

        Task<dynamic?> CreateFeeStructureAsync(FeeStructureRequestDto dto);
        Task<IEnumerable<dynamic>> GetFeeStructuresAsync();
        Task<dynamic?> GetFeeStructureByIdAsync(int id);
        Task<dynamic?> UpdateFeeStructureAsync(int id, FeeStructureRequestDto dto);
        Task<bool> DeleteFeeStructureAsync(int id);

        Task<dynamic?> AssignStudentFeeAsync(StudentFeeAssignmentRequestDto dto);
        Task<IEnumerable<dynamic>> GetStudentFeeDetailsAsync(int studentId);
        Task<dynamic?> GetStudentFeeAssignmentByIdAsync(int id);
        Task<dynamic?> UpdateStudentFeeAssignmentAsync(
            int id,
            StudentFeeAssignmentUpdateDto dto);
        Task<bool> AssignAdmissionFeesAsync(AdmissionFeeAssignDto request);

        Task<AdmissionFeeSummaryDto?> GetAdmissionFeeSummaryAsync(int admissionId);

        Task<AdmissionFeeSummaryDto?> CollectAdmissionFeeAsync(
            int admissionId,
            AdmissionFeePaymentDto request);
        Task<dynamic?> CollectFeeAsync(FeePaymentRequestDto dto);
        Task<IEnumerable<dynamic>> GetPaymentHistoryAsync(int studentId);
        Task<dynamic?> GetReceiptAsync(int receiptId);
        Task<bool> CancelPaymentAsync(int paymentId);

        Task<dynamic?> ApplyDiscountAsync(DiscountRequestDto dto);
        Task<dynamic?> ApplyScholarshipAsync(ScholarshipRequestDto dto);
        Task<dynamic?> ApplyFineAsync(FineRequestDto dto);
        Task<bool> WaiveFineAsync(int fineId);

        Task<dynamic?> CreateRefundAsync(RefundRequestDto dto);
        Task<IEnumerable<dynamic>> GetDueFeesAsync(int? studentId);
    }
}