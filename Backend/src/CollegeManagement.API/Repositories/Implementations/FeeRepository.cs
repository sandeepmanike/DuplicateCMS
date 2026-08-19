using CollegeManagement.API.Data;
using CollegeManagement.API.DTOs.Fee;
using CollegeManagement.API.Repositories.Interfaces;
using Dapper;
using Microsoft.EntityFrameworkCore;
using System.Data;

using System.Text.Json;



namespace CollegeManagement.API.Repositories.Implementations
{
    public class FeeRepository : IFeeRepository
    {
        private readonly AppDbContext _context;

        public FeeRepository(AppDbContext context)
        {
            _context = context;
        }

        private async Task<IDbConnection> GetConnectionAsync()
        {
            var connection = _context.Database.GetDbConnection();

            if (connection.State != ConnectionState.Open)
                await connection.OpenAsync();

            return connection;
        }

        public async Task<IEnumerable<dynamic>> GetFeeTypesAsync()
        {
            var connection = await GetConnectionAsync();

            return await connection.QueryAsync(
                "sp_GetFeeTypes",
                commandType: CommandType.StoredProcedure);
        }

        public async Task<dynamic?> CreateFeeStructureAsync(
            FeeStructureRequestDto dto)
        {
            var connection = await GetConnectionAsync();

            return await connection.QueryFirstOrDefaultAsync(
                "sp_CreateFeeStructure",
                new
                {
                    p_BoardId = dto.BoardId,
                    p_AcademicYearId = dto.AcademicYearId,
                    p_AcademicLevelId = dto.AcademicLevelId,
                    p_GroupId = dto.GroupId,
                    p_FeeTypeId = dto.FeeTypeId,
                    p_Amount = dto.Amount,
                    p_DueDate = dto.DueDate
                },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<dynamic>> GetFeeStructuresAsync()
        {
            var connection = await GetConnectionAsync();

            return await connection.QueryAsync(
                "sp_GetFeeStructures",
                commandType: CommandType.StoredProcedure);
        }

        public async Task<dynamic?> GetFeeStructureByIdAsync(int id)
        {
            var connection = await GetConnectionAsync();

            return await connection.QueryFirstOrDefaultAsync(
                "sp_GetFeeStructureById",
                new { p_FeeStructureId = id },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<dynamic?> UpdateFeeStructureAsync(
            int id,
            FeeStructureRequestDto dto)
        {
            var connection = await GetConnectionAsync();

            return await connection.QueryFirstOrDefaultAsync(
                "sp_UpdateFeeStructure",
                new
                {
                    p_FeeStructureId = id,
                    p_BoardId = dto.BoardId,
                    p_AcademicYearId = dto.AcademicYearId,
                    p_AcademicLevelId = dto.AcademicLevelId,
                    p_GroupId = dto.GroupId,
                    p_FeeTypeId = dto.FeeTypeId,
                    p_Amount = dto.Amount,
                    p_DueDate = dto.DueDate
                },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<bool> DeleteFeeStructureAsync(int id)
        {
            var connection = await GetConnectionAsync();

            var rows = await connection.ExecuteAsync(
                "sp_DeleteFeeStructure",
                new { p_FeeStructureId = id },
                commandType: CommandType.StoredProcedure);

            return rows > 0;
        }

        public async Task<dynamic?> AssignStudentFeeAsync(
            StudentFeeAssignmentRequestDto dto)
        {
            var connection = await GetConnectionAsync();

            return await connection.QueryFirstOrDefaultAsync(
                "sp_AssignStudentFee",
                new
                {
                    p_StudentId = dto.StudentId,
                    p_StudentAdmissionId = dto.StudentAdmissionId,
                    p_FeeStructureId = dto.FeeStructureId
                },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<dynamic>> GetStudentFeeDetailsAsync(
            int studentId)
        {
            var connection = await GetConnectionAsync();

            return await connection.QueryAsync(
                "sp_GetStudentFeeDetails",
                new { p_StudentId = studentId },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<dynamic?> GetStudentFeeAssignmentByIdAsync(int id)
        {
            var connection = await GetConnectionAsync();

            return await connection.QueryFirstOrDefaultAsync(
                "sp_GetStudentFeeAssignmentById",
                new { p_StudentFeeAssignmentId = id },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<dynamic?> UpdateStudentFeeAssignmentAsync(
            int id,
            StudentFeeAssignmentUpdateDto dto)
        {
            var connection = await GetConnectionAsync();

            return await connection.QueryFirstOrDefaultAsync(
                "sp_UpdateStudentFeeAssignment",
                new
                {
                    p_StudentFeeAssignmentId = id,
                    p_DiscountAmount = dto.DiscountAmount,
                    p_ScholarshipAmount = dto.ScholarshipAmount
                },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<dynamic?> CollectFeeAsync(
            FeePaymentRequestDto dto)
        {
            var connection = await GetConnectionAsync();

            return await connection.QueryFirstOrDefaultAsync(
                "sp_CollectStudentFee",
                new
                {
                    p_StudentFeeAssignmentId =
                        dto.StudentFeeAssignmentId,

                    p_Amount = dto.Amount,
                    p_PaymentMode = dto.PaymentMode,
                    p_TransactionNumber = dto.TransactionNumber,
                    p_Remarks = dto.Remarks
                },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<dynamic>> GetPaymentHistoryAsync(
            int studentId)
        {
            var connection = await GetConnectionAsync();

            return await connection.QueryAsync(
                "sp_GetPaymentHistory",
                new { p_StudentId = studentId },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<dynamic?> GetReceiptAsync(int receiptId)
        {
            var connection = await GetConnectionAsync();

            return await connection.QueryFirstOrDefaultAsync(
                "sp_GetFeeReceipt",
                new { p_PaymentId = receiptId },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<bool> CancelPaymentAsync(int paymentId)
        {
            var connection = await GetConnectionAsync();

            var rows = await connection.ExecuteAsync(
                "sp_CancelFeePayment",
                new { p_PaymentId = paymentId },
                commandType: CommandType.StoredProcedure);

            return rows > 0;
        }

        public async Task<dynamic?> ApplyDiscountAsync(
     DiscountRequestDto dto)
        {
            var connection = await GetConnectionAsync();

            return await connection.QueryFirstOrDefaultAsync(
                "sp_ApplyFeeDiscount",
                new
                {
                    p_AdmissionId = dto.AdmissionId,
                    p_DiscountAmount = dto.DiscountAmount,
                    p_Reason = dto.Reason
                },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<dynamic?> ApplyScholarshipAsync(
            ScholarshipRequestDto dto)
        {
            var connection = await GetConnectionAsync();

            return await connection.QueryFirstOrDefaultAsync(
                "sp_ApplyScholarship",
                new
                {
                    p_StudentFeeAssignmentId =
                        dto.StudentFeeAssignmentId,

                    p_ScholarshipAmount = dto.ScholarshipAmount,
                    p_ScholarshipName = dto.ScholarshipName
                },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<dynamic?> ApplyFineAsync(
            FineRequestDto dto)
        {
            var connection = await GetConnectionAsync();

            return await connection.QueryFirstOrDefaultAsync(
                "sp_ApplyFeeFine",
                new
                {
                    p_StudentFeeAssignmentId =
                        dto.StudentFeeAssignmentId,

                    p_FineAmount = dto.FineAmount,
                    p_Reason = dto.Reason
                },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<bool> WaiveFineAsync(int fineId)
        {
            var connection = await GetConnectionAsync();

            var rows = await connection.ExecuteAsync(
                "sp_WaiveFeeFine",
                new { p_FineId = fineId },
                commandType: CommandType.StoredProcedure);

            return rows > 0;
        }

        public async Task<dynamic?> CreateRefundAsync(
            RefundRequestDto dto)
        {
            var connection = await GetConnectionAsync();

            return await connection.QueryFirstOrDefaultAsync(
                "sp_CreateFeeRefund",
                new
                {
                    p_PaymentId = dto.PaymentId,
                    p_RefundAmount = dto.RefundAmount,
                    p_Reason = dto.Reason
                },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<dynamic>> GetDueFeesAsync(
            int? studentId)
        {
            var connection = await GetConnectionAsync();

            return await connection.QueryAsync(
                "sp_GetDueFees",
                new { p_StudentId = studentId },
                commandType: CommandType.StoredProcedure);
        }
        // =========================================================
        // ADMISSION FEE METHODS
        // =========================================================

        public async Task<bool> AssignAdmissionFeesAsync(
    AdmissionFeeAssignDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            if (dto.AdmissionId <= 0)
                throw new ArgumentException("Invalid AdmissionId.");

            if (dto.FeeStructureId <= 0)
                throw new ArgumentException("Invalid FeeStructureId.");

            if (dto.FeeItems == null || dto.FeeItems.Count == 0)
                throw new ArgumentException("FeeItems are required.");

            var connection = _context.Database.GetDbConnection();

            if (connection.State != ConnectionState.Open)
                await connection.OpenAsync();

            var feeItemsJson =
                System.Text.Json.JsonSerializer.Serialize(
                    dto.FeeItems,
                    new System.Text.Json.JsonSerializerOptions
                    {
                        PropertyNamingPolicy =
                            System.Text.Json.JsonNamingPolicy.CamelCase
                    });

            await connection.ExecuteAsync(
                "sp_AssignAdmissionFees",
                new
                {
                    p_AdmissionId = dto.AdmissionId,
                    p_FeeStructureId = dto.FeeStructureId,
                    p_FeeItemsJson = feeItemsJson
                },
                commandType: CommandType.StoredProcedure);

            return true;
        }


        public async Task<AdmissionFeeSummaryDto?> GetAdmissionFeeSummaryAsync(
            int admissionId)
        {
            var connection = await GetConnectionAsync();

            return await connection.QueryFirstOrDefaultAsync<AdmissionFeeSummaryDto>(
                "sp_GetAdmissionFeeSummary",
                new
                {
                    p_AdmissionId = admissionId
                },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<AdmissionFeeSummaryDto?> CollectAdmissionFeeAsync(
            int admissionId,
            AdmissionFeePaymentDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            var connection = await GetConnectionAsync();

            return await connection.QueryFirstOrDefaultAsync<AdmissionFeeSummaryDto>(
                "sp_CollectAdmissionFee",
                new
                {
                    p_AdmissionId = admissionId,
                    p_Amount = dto.Amount,
                    p_PaymentMode = dto.PaymentMode,
                    p_TransactionNumber = dto.TransactionNumber,
                    p_PaymentDate = dto.PaymentDate,
                    p_Remarks = dto.Remarks
                },
                commandType: CommandType.StoredProcedure);
        }
    }
}