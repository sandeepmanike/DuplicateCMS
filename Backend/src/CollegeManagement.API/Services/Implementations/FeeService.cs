using CollegeManagement.API.DTOs.Fee;
using CollegeManagement.API.Repositories.Interfaces;
using CollegeManagement.API.Services.Interfaces;

namespace CollegeManagement.API.Services.Implementations
{
    public class FeeService : IFeeService
    {
        private readonly IFeeRepository _feeRepository;

        public FeeService(IFeeRepository feeRepository)
        {
            _feeRepository = feeRepository;
        }

        public async Task<IEnumerable<dynamic>> GetFeeTypesAsync()
        {
            return await _feeRepository.GetFeeTypesAsync();
        }

        public async Task<dynamic?> CreateFeeStructureAsync(
            FeeStructureRequestDto dto)
        {
            ValidateFeeStructure(dto);

            return await _feeRepository
                .CreateFeeStructureAsync(dto);
        }

        public async Task<IEnumerable<dynamic>> GetFeeStructuresAsync()
        {
            return await _feeRepository.GetFeeStructuresAsync();
        }

        public async Task<dynamic?> GetFeeStructureByIdAsync(int id)
        {
            ValidateId(id);

            return await _feeRepository
                .GetFeeStructureByIdAsync(id);
        }

        public async Task<dynamic?> UpdateFeeStructureAsync(
            int id,
            FeeStructureRequestDto dto)
        {
            ValidateId(id);
            ValidateFeeStructure(dto);

            return await _feeRepository
                .UpdateFeeStructureAsync(id, dto);
        }

        public async Task<bool> DeleteFeeStructureAsync(int id)
        {
            ValidateId(id);

            return await _feeRepository
                .DeleteFeeStructureAsync(id);
        }

        public async Task<dynamic?> AssignStudentFeeAsync(
            StudentFeeAssignmentRequestDto dto)
        {
            if (dto.StudentId <= 0)
                throw new ArgumentException("Invalid StudentId.");

            if (dto.StudentAdmissionId <= 0)
                throw new ArgumentException(
                    "Invalid StudentAdmissionId.");

            if (dto.FeeStructureId <= 0)
                throw new ArgumentException(
                    "Invalid FeeStructureId.");

            return await _feeRepository
                .AssignStudentFeeAsync(dto);
        }

        public async Task<IEnumerable<dynamic>> GetStudentFeeDetailsAsync(
            int studentId)
        {
            ValidateId(studentId);

            return await _feeRepository
                .GetStudentFeeDetailsAsync(studentId);
        }

        public async Task<dynamic?> GetStudentFeeAssignmentByIdAsync(
            int id)
        {
            ValidateId(id);

            return await _feeRepository
                .GetStudentFeeAssignmentByIdAsync(id);
        }

        public async Task<dynamic?> UpdateStudentFeeAssignmentAsync(
            int id,
            StudentFeeAssignmentUpdateDto dto)
        {
            ValidateId(id);

            if (dto.DiscountAmount < 0)
                throw new ArgumentException(
                    "Discount cannot be negative.");

            if (dto.ScholarshipAmount < 0)
                throw new ArgumentException(
                    "Scholarship cannot be negative.");

            return await _feeRepository
                .UpdateStudentFeeAssignmentAsync(id, dto);
        }

        public async Task<dynamic?> CollectFeeAsync(
            FeePaymentRequestDto dto)
        {
            if (dto.StudentFeeAssignmentId <= 0)
                throw new ArgumentException(
                    "Invalid StudentFeeAssignmentId.");

            if (dto.Amount <= 0)
                throw new ArgumentException(
                    "Payment amount must be greater than zero.");

            if (string.IsNullOrWhiteSpace(dto.PaymentMode))
                throw new ArgumentException(
                    "Payment mode is required.");

            return await _feeRepository
                .CollectFeeAsync(dto);
        }

        public async Task<IEnumerable<dynamic>> GetPaymentHistoryAsync(
            int studentId)
        {
            ValidateId(studentId);

            return await _feeRepository
                .GetPaymentHistoryAsync(studentId);
        }

        public async Task<dynamic?> GetReceiptAsync(int receiptId)
        {
            ValidateId(receiptId);

            return await _feeRepository
                .GetReceiptAsync(receiptId);
        }

        public async Task<bool> CancelPaymentAsync(int paymentId)
        {
            ValidateId(paymentId);

            return await _feeRepository
                .CancelPaymentAsync(paymentId);
        }

        public async Task<dynamic?> ApplyDiscountAsync(
            DiscountRequestDto dto)
        {
            if (dto.AdmissionId <= 0)
                throw new ArgumentException(
                    "Invalid AdmissionId.");

            if (dto.DiscountAmount <= 0)
                throw new ArgumentException(
                    "Discount amount must be greater than zero.");

            return await _feeRepository
                .ApplyDiscountAsync(dto);
        }

        public async Task<dynamic?> ApplyScholarshipAsync(
            ScholarshipRequestDto dto)
        {
            if (dto.StudentFeeAssignmentId <= 0)
                throw new ArgumentException(
                    "Invalid StudentFeeAssignmentId.");

            if (dto.ScholarshipAmount <= 0)
                throw new ArgumentException(
                    "Scholarship amount must be greater than zero.");

            if (string.IsNullOrWhiteSpace(dto.ScholarshipName))
                throw new ArgumentException(
                    "Scholarship name is required.");

            return await _feeRepository
                .ApplyScholarshipAsync(dto);
        }

        public async Task<dynamic?> ApplyFineAsync(
            FineRequestDto dto)
        {
            if (dto.StudentFeeAssignmentId <= 0)
                throw new ArgumentException(
                    "Invalid StudentFeeAssignmentId.");

            if (dto.FineAmount <= 0)
                throw new ArgumentException(
                    "Fine amount must be greater than zero.");

            return await _feeRepository
                .ApplyFineAsync(dto);
        }

        public async Task<bool> WaiveFineAsync(int fineId)
        {
            ValidateId(fineId);

            return await _feeRepository
                .WaiveFineAsync(fineId);
        }

        public async Task<dynamic?> CreateRefundAsync(
            RefundRequestDto dto)
        {
            if (dto.PaymentId <= 0)
                throw new ArgumentException(
                    "Invalid PaymentId.");

            if (dto.RefundAmount <= 0)
                throw new ArgumentException(
                    "Refund amount must be greater than zero.");

            return await _feeRepository
                .CreateRefundAsync(dto);
        }

        public async Task<IEnumerable<dynamic>> GetDueFeesAsync(
            int? studentId)
        {
            if (studentId.HasValue && studentId.Value <= 0)
                throw new ArgumentException(
                    "Invalid StudentId.");

            return await _feeRepository
                .GetDueFeesAsync(studentId);
        }

        private static void ValidateId(int id)
        {
            if (id <= 0)
                throw new ArgumentException(
                    "ID must be greater than zero.");
        }

        private static void ValidateFeeStructure(
            FeeStructureRequestDto dto)
        {
            if (dto.BoardId <= 0)
                throw new ArgumentException("Invalid BoardId.");

            if (dto.AcademicYearId <= 0)
                throw new ArgumentException(
                    "Invalid AcademicYearId.");

            if (dto.AcademicLevelId <= 0)
                throw new ArgumentException(
                    "Invalid AcademicLevelId.");

            if (dto.GroupId <= 0)
                throw new ArgumentException(
                    "Invalid GroupId.");

            if (dto.FeeTypeId <= 0)
                throw new ArgumentException(
                    "Invalid FeeTypeId.");

            if (dto.Amount < 0)
                throw new ArgumentException(
                    "Amount cannot be negative.");
        }

        public async Task<bool> AssignAdmissionFeesAsync(
    AdmissionFeeAssignDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            if (dto.AdmissionId <= 0)
                throw new ArgumentException(
                    "Invalid AdmissionId.");

            if (dto.FeeStructureId <= 0)
                throw new ArgumentException(
                    "Invalid FeeStructureId.");

            if (dto.FeeItems == null || dto.FeeItems.Count == 0)
                throw new ArgumentException(
                    "At least one fee item is required.");

            foreach (var item in dto.FeeItems)
            {
                if (item.FeeTypeId <= 0)
                    throw new ArgumentException(
                        "Invalid FeeTypeId.");

                if (item.Amount < 0)
                    throw new ArgumentException(
                        "Fee amount cannot be negative.");
            }

            return await _feeRepository
                .AssignAdmissionFeesAsync(dto);
        }


        public async Task<AdmissionFeeSummaryDto?> GetAdmissionFeeSummaryAsync(
            int admissionId)
        {
            if (admissionId <= 0)
                throw new ArgumentException(
                    "Invalid AdmissionId.");

            return await _feeRepository
                .GetAdmissionFeeSummaryAsync(admissionId);
        }


        public async Task<AdmissionFeeSummaryDto?> CollectAdmissionFeeAsync(
            int admissionId,
            AdmissionFeePaymentDto dto)
        {
            if (admissionId <= 0)
                throw new ArgumentException(
                    "Invalid AdmissionId.");

            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            if (dto.Amount <= 0)
                throw new ArgumentException(
                    "Payment amount must be greater than zero.");

            if (string.IsNullOrWhiteSpace(dto.PaymentMode))
                throw new ArgumentException(
                    "Payment mode is required.");

            return await _feeRepository
                .CollectAdmissionFeeAsync(admissionId, dto);
        }


    }
}


