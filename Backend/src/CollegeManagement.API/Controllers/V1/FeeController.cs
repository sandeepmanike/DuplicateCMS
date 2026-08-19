using CollegeManagement.API.DTOs.Fee;
using CollegeManagement.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CollegeManagement.API.Controllers
{
    [ApiController]
    [Route("api/fees")]
    public class FeesController : ControllerBase
    {
        private readonly IFeeService _feeService;

        public FeesController(IFeeService feeService)
        {
            _feeService = feeService;
        }

        // =====================================================
        // 1. GET FEE TYPES
        // =====================================================

        [HttpGet("types")]
        public async Task<IActionResult> GetFeeTypes()
        {
            try
            {
                var result = await _feeService.GetFeeTypesAsync();

                return Ok(new
                {
                    success = true,
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }


        // =====================================================
        // 2. CREATE FEE STRUCTURE
        // =====================================================

        [HttpPost("structure")]
        public async Task<IActionResult> CreateFeeStructure(
            [FromBody] FeeStructureRequestDto dto)
        {
            try
            {
                var result =
                    await _feeService.CreateFeeStructureAsync(dto);

                return Ok(new
                {
                    success = true,
                    message = "Fee structure created successfully.",
                    data = result
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }


        // =====================================================
        // 3. GET ALL FEE STRUCTURES
        // =====================================================

        [HttpGet("structure")]
        public async Task<IActionResult> GetFeeStructures()
        {
            try
            {
                var result =
                    await _feeService.GetFeeStructuresAsync();

                return Ok(new
                {
                    success = true,
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }


        // =====================================================
        // 4. GET FEE STRUCTURE BY ID
        // =====================================================

        [HttpGet("structure/{id:int}")]
        public async Task<IActionResult> GetFeeStructureById(int id)
        {
            try
            {
                var result =
                    await _feeService.GetFeeStructureByIdAsync(id);

                if (result == null)
                    return NotFound(new
                    {
                        success = false,
                        message = "Fee structure not found."
                    });

                return Ok(new
                {
                    success = true,
                    data = result
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }


        // =====================================================
        // 5. UPDATE FEE STRUCTURE
        // =====================================================

        [HttpPut("structure/{id:int}")]
        public async Task<IActionResult> UpdateFeeStructure(
            int id,
            [FromBody] FeeStructureRequestDto dto)
        {
            try
            {
                var result =
                    await _feeService
                        .UpdateFeeStructureAsync(id, dto);

                if (result == null)
                    return NotFound(new
                    {
                        success = false,
                        message = "Fee structure not found."
                    });

                return Ok(new
                {
                    success = true,
                    message = "Fee structure updated successfully.",
                    data = result
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }


        // =====================================================
        // 6. DELETE FEE STRUCTURE
        // =====================================================

        [HttpDelete("structure/{id:int}")]
        public async Task<IActionResult> DeleteFeeStructure(int id)
        {
            try
            {
                var result =
                    await _feeService.DeleteFeeStructureAsync(id);

                if (!result)
                    return NotFound(new
                    {
                        success = false,
                        message = "Fee structure not found."
                    });

                return Ok(new
                {
                    success = true,
                    message = "Fee structure deactivated successfully."
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }


        // =====================================================
        // 7. ASSIGN FEE TO STUDENT
        // =====================================================

        [HttpPost("assign")]
        public async Task<IActionResult> AssignStudentFee(
            [FromBody] StudentFeeAssignmentRequestDto dto)
        {
            try
            {
                var result =
                    await _feeService.AssignStudentFeeAsync(dto);

                return Ok(new
                {
                    success = true,
                    message = "Fee assigned to student successfully.",
                    data = result
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }


        // =====================================================
        // 8. GET STUDENT FEE DETAILS
        // =====================================================

        [HttpGet("student/{studentId:int}")]
        public async Task<IActionResult> GetStudentFeeDetails(
            int studentId)
        {
            try
            {
                var result =
                    await _feeService
                        .GetStudentFeeDetailsAsync(studentId);

                if (result == null || !result.Any())
                    return NotFound(new
                    {
                        success = false,
                        message = "No fee details found."
                    });

                return Ok(new
                {
                    success = true,
                    data = result
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }


        // =====================================================
        // 9. GET ASSIGNMENT BY ID
        // =====================================================

        [HttpGet("assignment/{id:int}")]
        public async Task<IActionResult>
            GetStudentFeeAssignmentById(int id)
        {
            try
            {
                var result =
                    await _feeService
                        .GetStudentFeeAssignmentByIdAsync(id);

                if (result == null)
                    return NotFound(new
                    {
                        success = false,
                        message = "Fee assignment not found."
                    });

                return Ok(new
                {
                    success = true,
                    data = result
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }


        // =====================================================
        // 10. UPDATE STUDENT FEE ASSIGNMENT
        // =====================================================

        [HttpPut("assignment/{id:int}")]
        public async Task<IActionResult>
            UpdateStudentFeeAssignment(
                int id,
                [FromBody] StudentFeeAssignmentUpdateDto dto)
        {
            try
            {
                var result =
                    await _feeService
                        .UpdateStudentFeeAssignmentAsync(id, dto);

                if (result == null)
                    return NotFound(new
                    {
                        success = false,
                        message = "Fee assignment not found."
                    });

                return Ok(new
                {
                    success = true,
                    message = "Fee assignment updated successfully.",
                    data = result
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }


        // =====================================================
        // 11. COLLECT FEE
        // =====================================================

        [HttpPost("collect")]
        public async Task<IActionResult> CollectFee(
            [FromBody] FeePaymentRequestDto dto)
        {
            try
            {
                var result =
                    await _feeService.CollectFeeAsync(dto);

                return Ok(new
                {
                    success = true,
                    message = "Fee payment completed successfully.",
                    data = result
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }


        // =====================================================
        // 12. PAYMENT HISTORY
        // =====================================================

        [HttpGet("history/{studentId:int}")]
        public async Task<IActionResult>
            GetPaymentHistory(int studentId)
        {
            try
            {
                var result =
                    await _feeService
                        .GetPaymentHistoryAsync(studentId);

                return Ok(new
                {
                    success = true,
                    data = result
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }


        // =====================================================
        // 13. RECEIPT
        // =====================================================

        [HttpGet("receipt/{receiptId:int}")]
        public async Task<IActionResult>
            GetReceipt(int receiptId)
        {
            try
            {
                var result =
                    await _feeService.GetReceiptAsync(receiptId);

                if (result == null)
                    return NotFound(new
                    {
                        success = false,
                        message = "Receipt not found."
                    });

                return Ok(new
                {
                    success = true,
                    data = result
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }


        // =====================================================
        // 14. CANCEL PAYMENT
        // =====================================================

        [HttpDelete("payment/{id:int}")]
        public async Task<IActionResult>
            CancelPayment(int id)
        {
            try
            {
                var result =
                    await _feeService.CancelPaymentAsync(id);

                if (!result)
                    return NotFound(new
                    {
                        success = false,
                        message = "Payment not found."
                    });

                return Ok(new
                {
                    success = true,
                    message = "Payment cancelled successfully."
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }


        // =====================================================
        // 15. DISCOUNT
        // =====================================================

        [HttpPost("discount")]
        public async Task<IActionResult> ApplyDiscount(
    [FromBody] DiscountRequestDto dto)
        {
            try
            {
                if (dto == null)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Request is required."
                    });
                }

                if (dto.AdmissionId <= 0)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Valid AdmissionId is required."
                    });
                }

                if (dto.DiscountAmount <= 0)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Discount amount must be greater than zero."
                    });
                }

                var result = await _feeService.ApplyDiscountAsync(dto);

                return Ok(new
                {
                    success = true,
                    message = "Discount applied successfully.",
                    data = result
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        // =====================================================
        // 16. SCHOLARSHIP
        // =====================================================

        [HttpPost("scholarship")]
        public async Task<IActionResult> ApplyScholarship(
            [FromBody] ScholarshipRequestDto dto)
        {
            try
            {
                var result =
                    await _feeService.ApplyScholarshipAsync(dto);

                return Ok(new
                {
                    success = true,
                    message = "Scholarship applied successfully.",
                    data = result
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }


        // =====================================================
        // 17. FINE
        // =====================================================

        [HttpPost("fine")]
        public async Task<IActionResult> ApplyFine(
            [FromBody] FineRequestDto dto)
        {
            try
            {
                var result =
                    await _feeService.ApplyFineAsync(dto);

                return Ok(new
                {
                    success = true,
                    message = "Fine applied successfully.",
                    data = result
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }


        // =====================================================
        // 18. WAIVE FINE
        // =====================================================

        [HttpPatch("fine/{id:int}/waive")]
        public async Task<IActionResult> WaiveFine(int id)
        {
            try
            {
                var result =
                    await _feeService.WaiveFineAsync(id);

                if (!result)
                    return NotFound(new
                    {
                        success = false,
                        message = "Fine not found."
                    });

                return Ok(new
                {
                    success = true,
                    message = "Fine waived successfully."
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }


        // =====================================================
        // 19. REFUND
        // =====================================================

        [HttpPost("refund")]
        public async Task<IActionResult> CreateRefund(
            [FromBody] RefundRequestDto dto)
        {
            try
            {
                var result =
                    await _feeService.CreateRefundAsync(dto);

                return Ok(new
                {
                    success = true,
                    message = "Refund processed successfully.",
                    data = result
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }


        // =====================================================
        // 20. DUE FEES
        // =====================================================

        [HttpGet("due")]
        public async Task<IActionResult> GetDueFees(
     [FromQuery] int? studentId)
        {
            try
            {
                var result =
                    await _feeService.GetDueFeesAsync(studentId);

                return Ok(new
                {
                    success = true,
                    data = result
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }


        // =====================================================
        // ADMISSION FEE APIs
        // =====================================================

        [HttpPost("admission/assign")]
        public async Task<IActionResult> AssignAdmissionFees(
     AdmissionFeeAssignDto dto)
        {
            try
            {
                var result = await _feeService.AssignAdmissionFeesAsync(dto);

                return Ok(new
                {
                    success = true,
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = ex.Message,
                    innerException = ex.InnerException?.Message,
                    details = ex.ToString()
                });
            }
        }

        [HttpGet("admission/{admissionId}/summary")]
        public async Task<IActionResult> GetAdmissionFeeSummary(
            int admissionId)
        {
            if (admissionId <= 0)
                return BadRequest(new
                {
                    message = "Valid AdmissionId is required"
                });

            var result =
                await _feeService.GetAdmissionFeeSummaryAsync(admissionId);

            if (result == null)
                return NotFound(new
                {
                    message = "Admission fee details not found"
                });

            return Ok(result);
        }


        [HttpPost("admission/{admissionId}/payment")]
        public async Task<IActionResult> CollectAdmissionFee(
            int admissionId,
            [FromBody] AdmissionFeePaymentDto request)
        {
            if (admissionId <= 0)
                return BadRequest(new
                {
                    message = "Valid AdmissionId is required"
                });

            if (request == null)
                return BadRequest(new
                {
                    message = "Payment request is required"
                });

            if (request.Amount <= 0)
                return BadRequest(new
                {
                    message = "Payment amount must be greater than zero"
                });

            if (string.IsNullOrWhiteSpace(request.PaymentMode))
                return BadRequest(new
                {
                    message = "Payment mode is required"
                });

            var result =
                await _feeService.CollectAdmissionFeeAsync(
                    admissionId,
                    request);

            if (result == null)
                return BadRequest(new
                {
                    message = "Unable to collect admission fee"
                });

            return Ok(new
            {
                message = "Admission fee paid successfully",
                data = result
            });
        }
    }
}