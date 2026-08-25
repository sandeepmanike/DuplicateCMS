using System.Threading.Tasks;
using Asp.Versioning;
using CollegeManagement.API.DTOs.Staff;
using CollegeManagement.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CollegeManagement.API.Controllers.V1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/staff")]
    [Authorize]
    [Produces("application/json")]
    public class StaffSubjectAllocationController : ControllerBase
    {
        private readonly IStaffService _staffService;

        public StaffSubjectAllocationController(IStaffService staffService)
        {
            _staffService = staffService;
        }

        /// <summary>
        /// 1. POST /api/v1/staff/assign-subject
        /// Assign a subject to a teaching staff member.
        /// </summary>
        [HttpPost("assign-subject")]
        [ProducesResponseType(typeof(StaffSubjectAllocationResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> AssignSubject([FromBody] AssignStaffSubjectDto dto)
        {
            var result = await _staffService.AssignSubjectAsync(dto);
            return CreatedAtAction("GetStaffById", "Staff", new { id = dto.StaffId }, result);
        }

        /// <summary>
        /// 2. PUT /api/v1/staff/assign-subject/{id:int}
        /// Update an existing subject allocation.
        /// </summary>
        [HttpPut("assign-subject/{id:int}")]
        [ProducesResponseType(typeof(StaffSubjectAllocationResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> UpdateSubjectAllocation(int id, [FromBody] UpdateStaffSubjectAllocationDto dto)
        {
            var result = await _staffService.UpdateSubjectAllocationAsync(id, dto);
            return Ok(result);
        }

        /// <summary>
        /// 3. DELETE /api/v1/staff/assign-subject/{id:int}
        /// Delete a subject allocation record.
        /// </summary>
        [HttpDelete("assign-subject/{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteSubjectAllocation(int id)
        {
            await _staffService.DeleteSubjectAllocationAsync(id);
            return NoContent();
        }

        /// <summary>
        /// 4. GET /api/v1/staff/{staffId:int}/subject-allocations
        /// Get all subject allocations for a specific staff member.
        /// </summary>
        [HttpGet("{staffId:int}/subject-allocations")]
        [ProducesResponseType(typeof(System.Collections.Generic.List<StaffSubjectAllocationResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetStaffSubjectAllocations(int staffId)
        {
            var result = await _staffService.GetStaffSubjectAllocationsAsync(staffId);
            return Ok(result);
        }

        /// <summary>
        /// 5. GET /api/v1/staff/workload/{staffId:int}
        /// Get summary workload details and subject allocations for a staff member.
        /// </summary>
        [HttpGet("workload/{staffId:int}")]
        [ProducesResponseType(typeof(StaffWorkloadResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetStaffWorkload(int staffId)
        {
            var result = await _staffService.GetStaffWorkloadAsync(staffId);
            return Ok(result);
        }
    }
}
