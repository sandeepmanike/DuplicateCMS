using CollegeManagement.API.DTOs.Promotion;
using CollegeManagement.API.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CollegeManagement.API.Controllers
{
    [ApiController]
    [Route("api/v1/promotions")]
    [Produces("application/json")]
    public class PromotionController : ControllerBase
    {     
        private readonly IPromotionService _promotionService;

        public PromotionController(IPromotionService promotionService)
        {
            _promotionService = promotionService;
        }

        /// <summary>
        /// Get Eligible Students
        /// GET: api/v1/promotions/eligible
        /// </summary>
        [HttpGet("eligible")]
        [ProducesResponseType(typeof(List<EligibleStudentDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetEligibleStudents()
        {
            var result = await _promotionService.GetEligibleStudentsAsync();

            return Ok(result);
        }

        /// <summary>
        /// Promote Multiple Students
        /// POST: api/v1/promotions
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(PromotionResponseDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> PromoteStudents(
            [FromBody] PromotionRequestDto dto)
        {
            var result = await _promotionService.PromoteStudentsAsync(dto);

            return Ok(result);
        }

        /// <summary>
        /// Promotion History
        /// GET: api/v1/promotions/history
        /// </summary>
        [HttpGet("history")]
        [ProducesResponseType(typeof(List<PromotionHistoryDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPromotionHistory()
        {
            var result =
                await _promotionService.GetPromotionHistoryAsync();

            return Ok(result);
        }

        /// <summary>
        /// Rollback Promotion
        /// POST: api/v1/promotions/rollback
        /// </summary>
        [HttpPost("rollback")]
        [ProducesResponseType(typeof(PromotionResponseDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> RollbackPromotion(
            [FromBody] RollbackPromotionDto dto)
        {
            var result =
                await _promotionService.RollbackPromotionAsync(dto);

            return Ok(result);
        }

        /// <summary>
        /// Promote Single Student
        /// POST: api/v1/promotions/student/{studentId}
        /// </summary>
        [HttpPost("student/{studentId:int}")]
        [ProducesResponseType(typeof(PromotionResponseDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> PromoteSingleStudent(
            int studentId)
        {
            var result =
                await _promotionService
                    .PromoteSingleStudentAsync(studentId);

            return Ok(result);
        }

        /// <summary>
        /// Promotion Report
        /// GET: api/v1/promotions/report
        /// </summary>
        [HttpGet("report")]
        [ProducesResponseType(typeof(PromotionReportDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPromotionReport()
        {
            var result =
                await _promotionService.GetPromotionReportAsync();

            return Ok(result);
        }

        /// <summary>
        /// Section Allocation
        /// PATCH: api/v1/promotions/section-allocation
        /// </summary>
        [HttpPatch("section-allocation")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateSectionAllocation(
            [FromBody] SectionAllocationDto dto)
        {
            var result =
                await _promotionService.UpdateSectionAllocationAsync(dto);

            return Ok(new
            {
                Success = result,
                Message = "Section allocated successfully."
            });
        }

        /// <summary>
        /// Group Allocation
        /// PATCH: api/v1/promotions/group-allocation
        /// </summary>
        [HttpPatch("group-allocation")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateGroupAllocation(
            [FromBody] GroupAllocationDto dto)
        {
            var result =
                await _promotionService.UpdateGroupAllocationAsync(dto);

            return Ok(new
            {
                Success = result,
                Message = "Group allocated successfully."
            });
        }
    }
}