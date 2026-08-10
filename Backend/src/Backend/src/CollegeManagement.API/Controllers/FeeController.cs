using CollegeManagement.API.DTOs.Fee;
using CollegeManagement.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CollegeManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FeeController : ControllerBase
    {
        private readonly IFeeService _feeService;

        public FeeController(IFeeService feeService)
        {
            _feeService = feeService;
        }

        [HttpPost("structure")]
        public async Task<IActionResult> CreateFeeStructure([FromBody] CreateFeeStructureDto dto)
        {
            var result = await _feeService.CreateFeeStructureAsync(dto);
            return Ok(result);
        }

        [HttpGet("structure")]
        public async Task<IActionResult> GetAllFeeStructures()
        {
            var result = await _feeService.GetAllFeeStructuresAsync();
            return Ok(result);
        }

        

        [HttpPut("structure/{id}")]
        public async Task<IActionResult> UpdateFeeStructure(int id, [FromBody] UpdateFeeStructureDto dto)
        {
            var result = await _feeService.UpdateFeeStructureAsync(id, dto);
            if (result == null) return NotFound();
            return Ok(result);
        }

    

        [HttpPost("assign")]
        public async Task<IActionResult> AssignFeeToStudent([FromBody] AssignFeeDto dto)
        {
            var result = await _feeService.AssignFeeToStudentAsync( dto);
            return Ok(result);
        }

        [HttpPost("collect")]
        public async Task<IActionResult> CollectFee([FromBody] CreateFeeCollectionDto dto)
        {
            var result = await _feeService.CollectFeeAsync(dto);
            return Ok(result);
        }

        [HttpGet("student/{studentId}")]
        public async Task<IActionResult> GetStudentFeeDetails(int studentId)
        {
            var result = await _feeService.GetStudentFeeDetailsAsync(studentId);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPut("payment/{id}")]
        public async Task<IActionResult> UpdatePayment(int id, [FromBody] UpdatePaymentDto dto)
        {
            var result = await _feeService.UpdatePaymentAsync(id, dto);
            return Ok(result);
        }

        [HttpPost("discount")]
        public async Task<IActionResult> ApplyDiscount([FromBody] ApplyDiscountDto dto)
        {
            await _feeService.ApplyDiscountAsync(dto);
            return Ok(new { message = "Discount Applied" });
        }

        [HttpPost("fine")]
        public async Task<IActionResult> ApplyFine([FromBody] ApplyFineDto dto)
        {
            await _feeService.ApplyFineAsync(dto);
            return Ok(new { message = "Fine Applied" });
        }

        [HttpPatch("fine/{id}/waive")]
        public async Task<IActionResult> WaiveFine(int id)
        {
            await _feeService.WaiveFineAsync(id);
            return Ok(new { message = "Fine waived successfully" });
        }
        [HttpGet("receipt/{receiptId}")]
        public async Task<IActionResult> GenerateReceipt(string receiptId)
        {
            var result = await _feeService.GenerateReceiptAsync(receiptId);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpPost("scholarship")]
        public async Task<IActionResult> ApplyScholarship([FromBody] ApplyScholarshipDto dto)
        {
            await _feeService.ApplyScholarshipAsync(dto);
            return Ok(new { message = "Scholarship Applied" });
        }

        [HttpPost("refund")]
        public async Task<IActionResult> RefundFee([FromBody] RefundFeeDto dto)
        {
            await _feeService.RefundFeeAsync(dto);
            return Ok(new { message = "Refund Processed" });
        }

        [HttpGet("due")]
        public async Task<IActionResult> GetDueFees()
        {
            var result = await _feeService.GetDueFeesAsync();
            return Ok(result);
        }

        

            [HttpGet("history/{studentId}")]

            public async Task<IActionResult> GetFeePaymentHistory(int studentId)
            {
                var result = await _feeService.GetFeePaymentHistoryAsync(studentId);
                return Ok(result);
            }

          

        [HttpGet("receipt/download/{feeCollectionId}")]
            public async Task<IActionResult> DownloadFeeReceipt(int feeCollectionId)
            {
                var pdfBytes = await _feeService.DownloadFeeReceiptAsync(feeCollectionId);
                return File(pdfBytes, "application/pdf", $"Receipt_{feeCollectionId}.pdf");
            }
        [HttpDelete("payment/{id}")]
public async Task<IActionResult> CancelPayment(int id)
        {
            var result = await _feeService.CancelPaymentAsync(id);

            if (!result)
                return NotFound();

            return Ok(new { message = "Payment cancelled successfully" });
        }

        [HttpGet("report/daily")]
        public async Task<IActionResult> GetDailyCollection([FromQuery] DateTime date)
        {
            var result = await _feeService.GetDailyCollectionAsync(date);
            return Ok(result);
        }

        [HttpGet("report/monthly")]
        public async Task<IActionResult> GetMonthlyCollection([FromQuery] int month, [FromQuery] int year)
        {
            var result = await _feeService.GetMonthlyCollectionAsync(month, year);
            return Ok(result);
        }

        [HttpGet("report/outstanding")]
        public async Task<IActionResult> GetOutstandingReport()
        {
            var result = await _feeService.GetOutstandingReportAsync();
            return Ok(result);
        }
    }
    }
