using CollegeManagement.API.Data;
using CollegeManagement.API.DTOs.Fee;
using CollegeManagement.API.Models;
using CollegeManagement.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CollegeManagement.API.Services.Implementations
{
    public class FeeService : IFeeService
    {
        private readonly AppDbContext _context;
        public FeeService(AppDbContext context) => _context = context;

        // ---------------- Fee Structure ----------------
        public async Task<FeeStructure> CreateFeeStructureAsync(CreateFeeStructureDto dto)
        {
            var fee = new FeeStructure
            {
                BoardId = dto.BoardId,
                AcademicYearId = dto.AcademicYearId,
                GroupId = dto.GroupId,
                FeeType = dto.FeeType,
                Amount = dto.Amount,
                DueDate = dto.DueDate,
                IsActive = true
            };
            _context.FeeStructures.Add(fee);
            await _context.SaveChangesAsync();
            return fee;
        }


        public async Task<FeeCollection> AssignFeeToStudentAsync(AssignFeeDto dto)
        {
            var fee = new FeeCollection
            {
                StudentId = dto.StudentId,
                FeeStructureId = dto.FeeStructureId,
                Amount = 0,
                Status = "Pending",
                PaymentDate = DateTime.Now
            };
            _context.FeeCollections.Add(fee);
            var result = await _context.SaveChangesAsync();
            return fee;
        }
        public async Task<FeeStructure?> UpdateFeeStructureAsync(int id, UpdateFeeStructureDto dto)
        {
            var fee = await _context.FeeStructures.FindAsync(id);
            if (fee == null)
                return null;

            fee.BoardId = dto.BoardId;
            fee.AcademicYearId = dto.AcademicYearId;
            fee.GroupId = dto.GroupId;
            fee.FeeType = dto.FeeType;
            fee.Amount = dto.Amount;
            fee.DueDate = dto.DueDate;

            await _context.SaveChangesAsync();
            return fee;
        }

        public async Task<IEnumerable<FeeStructure>> GetAllFeeStructuresAsync()
        {
            return await _context.FeeStructures.Where(x => x.IsActive).ToListAsync();
        }

        public async Task<FeeStructure?> GetFeeStructureByIdAsync(int id)
        {
            return await _context.FeeStructures.FindAsync(id);
        }

        public async Task<bool> DeleteFeeStructureAsync(int id)
        {
            var fee = await _context.FeeStructures.FindAsync(id);
            if (fee == null) return false;

            _context.FeeStructures.Remove(fee);
            await _context.SaveChangesAsync();
            return true;
        }

        // ---------------- Fee Collection ----------------
        public async Task<FeeCollection> CollectFeeAsync(CreateFeeCollectionDto dto)
        {
            var payment = new FeeCollection
            {
                StudentId = dto.StudentId,
                FeeStructureId = dto.FeeStructureId,
                Amount = dto.Amount,
                PaymentMode = "",
                PaymentDate = DateTime.Now,
                ReceiptId = "RCP" + DateTime.Now.Ticks,
                Status = "Pending",
                Discount = dto.Discount,
                Fine = dto.Fine,
                TransactionId = "",
                DueAmount = 0,

                TransactionNumber = "",
            };
            _context.FeeCollections.Add(payment);
            await _context.SaveChangesAsync();
            return payment;
        }
      

        public async Task<IEnumerable<FeeCollection>> GetStudentFeeDetailsAsync(int studentId)
        {
            return await _context.FeeCollections
                .Where(x => x.StudentId == studentId)
                .ToListAsync();
        }

        public async Task<FeeCollection?> UpdatePaymentAsync(int id, UpdatePaymentDto dto)
        {
            var payment = await _context.FeeCollections.FindAsync(id);
            if (payment == null) return null;

            payment.Amount = dto.Amount;
            payment.PaymentMode = dto.PaymentMode ?? string.Empty;
            await _context.SaveChangesAsync();
            return payment;
        }

        public async Task<bool> CancelPaymentAsync(int id)
        {
            var payment = await _context.FeeCollections.FindAsync(id);
            if (payment == null) return false;

            payment.Status = "Cancelled";
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task RefundFeeAsync(RefundFeeDto dto)
        {
            var payment = await _context.FeeCollections.FindAsync(dto.FeeCollectionId);
            if (payment == null) return;

            payment.Status = "Refunded";
            await _context.SaveChangesAsync();
        }

        public async Task ApplyDiscountAsync(ApplyDiscountDto dto)
        {
            var payment = await _context.FeeCollections.FindAsync(dto.FeeCollectionId);
            if (payment == null) return;

            payment.Discount = dto.DiscountAmount;
            await _context.SaveChangesAsync();
        }

        public async Task ApplyScholarshipAsync(ApplyScholarshipDto dto)
        {
            // Scholarship logic placeholder
            await Task.CompletedTask;
        }

        public async Task ApplyFineAsync(ApplyFineDto dto)
        {
            var payment = await _context.FeeCollections.FindAsync(dto.FeeCollectionId);
            if (payment == null) return;

            payment.Fine = dto.FineAmount;
            await _context.SaveChangesAsync();
        }

        public async Task WaiveFineAsync(int id)
        {
            var payment = await _context.FeeCollections.FindAsync(id);
            if (payment == null) return;

            payment.Fine = 0;
            await _context.SaveChangesAsync();
        }
        public async Task<object> GetDueFeeByYearAsync(int academicYear)
        {
            
            var result = await _context.FeeCollections
                .Where(f => f.FeeStructure.AcademicYearId == academicYear && f.Status == "Pending")
                .Include(f => f.Student)
                .Include(f => f.FeeStructure)
                .ToListAsync();

            return result;
        }


        // ---------------- Reports ----------------
        public async Task<object?> GenerateReceiptAsync(string receiptId)
        {
            var payment = await _context.FeeCollections
                .FirstOrDefaultAsync(x => x.ReceiptId == receiptId);
            return payment;
        }

        public async Task<object> GetFeeDefaulterReportAsync()
        {
            return await _context.StudentFees
                .Where(x => x.DueAmount > 0)
                .ToListAsync();
        }
        public async Task<byte[]> DownloadReceiptAsync(string receiptId)
        {
            return await Task.FromResult(new byte[0]);
        }

        public async Task<byte[]> DownloadFeeReceiptAsync(int feeCollectionId)
        {
            return await Task.FromResult(new byte[0]);
        }

        public async Task<IEnumerable<FeeCollection>> GetPaymentHistoryAsync(int studentId)
        {
            return await _context.FeeCollections
                .Where(x => x.StudentId == studentId)
                .ToListAsync();
        }

        public async Task<object> GetDailyCollectionAsync(DateTime date)
        {
            var total = await _context.FeeCollections
               .Where(x => x.PaymentDate.Date == date && x.Status == "Paid")
               .SumAsync(x => x.Amount + x.Fine - x.Discount);
            return new { Date = date, TotalCollection = total };
        }

        public async Task<object> GetMonthlyCollectionAsync(int month, int year)
        {
            var total = await _context.FeeCollections
               .Where(x => x.PaymentDate.Month == month && x.PaymentDate.Year == year && x.Status == "Paid")
               .SumAsync(x => x.Amount + x.Fine - x.Discount);
            return new { Month = month, Year = year, TotalCollection = total };
        }

        public async Task<object> GetOutstandingReportAsync()
        {
            var totalOutstanding = await _context.FeeCollections
               .Where(x => x.Status != "Paid")
               .SumAsync(x => x.Amount);
            return new { TotalOutstanding = totalOutstanding };
        }

        public async Task<IEnumerable<FeeCollection>> GetPendingFeesReportAsync()
        {
            return await _context.FeeCollections
                .Where(x => x.Status != "Paid")
                .ToListAsync();
        }

        public async Task<IEnumerable<FeeCollection>> GetCollectedFeesReportAsync(DateTime from, DateTime to)
        {
            return await _context.FeeCollections
                .Where(x => x.PaymentDate >= from &&
                            x.PaymentDate <= to &&
                            x.Status == "Paid")
                .ToListAsync();
        }
        public async Task<IEnumerable<FeePaymentHistoryDto>> GetFeePaymentHistoryAsync(int studentId)
        {
            return await _context.FeeCollections
                .Where(f => f.StudentId == studentId && f.Status == "Paid")
                .Include(f => f.FeeStructure)
                .Include(f => f.FeeStructure.AcademicYear)
                .Select(f => new FeePaymentHistoryDto
                {
                    ReceiptId = f.ReceiptId ?? "",
                    FeeType = f.FeeStructure.FeeType,
                    Amount = f.Amount,
                    PaymentDate = f.PaymentDate,
                    AcademicYear = f.FeeStructure.AcademicYear.AcademicYearName
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<FeeCollection>> GetDueFeesAsync(int studentId)
        {
            return await _context.FeeCollections
                .Where(x => x.StudentId == studentId && x.Status != "Pending")
                .ToListAsync();
        }
        public async Task<FeeCollection?> GetDueFeeAsync(int studentId) 
        {
            return await _context.FeeCollections
                .FirstOrDefaultAsync(f => f.StudentId == studentId && (f.Status == "Pending" || f.Amount > 0));
        }
        public async Task<IEnumerable<FeeCollection>> GetDueFeesAsync()
        {
            return await _context.FeeCollections
                .Where(x => x.Status != "Paid")
                .ToListAsync();
        }
        public async Task<IEnumerable<Student>> GetDueFeeStudentsAsync(int academicYearId)
        {
            var studentIds = await _context.FeeCollections
                .Include(f => f.FeeStructure)
                .Where(f => f.FeeStructure.AcademicYearId == academicYearId && f.Status == "Pending")
                .Select(f => f.StudentId)
                .Distinct()
                .ToListAsync();

            return await _context.Students
                .Where(s => studentIds.Contains(s.StudentId))
                .ToListAsync();
        }



    }
    }
    

