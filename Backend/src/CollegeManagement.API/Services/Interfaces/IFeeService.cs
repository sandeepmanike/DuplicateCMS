using CollegeManagement.API.DTOs.Fee;
using CollegeManagement.API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CollegeManagement.API.Services.Interfaces
{
    public interface IFeeService
    {
        // Fee Structure
        Task<FeeStructure> CreateFeeStructureAsync(CreateFeeStructureDto dto);
        Task<FeeStructure?> UpdateFeeStructureAsync(int id, UpdateFeeStructureDto dto);
        Task<IEnumerable<FeeStructure>> GetAllFeeStructuresAsync();
        
        Task<object?> GenerateReceiptAsync(string receiptId);
        

        // Fee Collection
        Task<FeeCollection> CollectFeeAsync(CreateFeeCollectionDto dto);
        Task<IEnumerable<FeeCollection>> GetStudentFeeDetailsAsync(int studentId);
        Task<FeeCollection?> UpdatePaymentAsync(int id, UpdatePaymentDto dto);
        Task<bool> CancelPaymentAsync(int id);
        Task RefundFeeAsync(RefundFeeDto dto);

        // Discounts, Scholarships, Fines
        Task ApplyDiscountAsync(ApplyDiscountDto dto);
        Task ApplyScholarshipAsync(ApplyScholarshipDto dto);
        Task ApplyFineAsync(ApplyFineDto dto);
        Task WaiveFineAsync(int id);

        // Reports
        
        Task<byte[]> DownloadReceiptAsync(string receiptId);
        Task<IEnumerable<FeeCollection>> GetDueFeesAsync(int studentId);
        
        Task<FeeCollection?> GetDueFeeAsync(int studentId);
        Task<IEnumerable<FeeCollection>> GetPaymentHistoryAsync(int studentId);
        Task<object> GetDailyCollectionAsync(DateTime date);
        Task<object> GetMonthlyCollectionAsync(int month, int year);
        Task<object> GetOutstandingReportAsync();
        
        
        
        
        Task<FeeCollection> AssignFeeToStudentAsync(AssignFeeDto dto);
       
       
        Task<byte[]> DownloadFeeReceiptAsync(int feeCollectionId);
        Task<IEnumerable<FeePaymentHistoryDto>> GetFeePaymentHistoryAsync(int studentId);
        Task<IEnumerable<Student>> GetDueFeeStudentsAsync(int academicYearId);
        Task<IEnumerable<FeeCollection>> GetDueFeesAsync();
        



    }
}
