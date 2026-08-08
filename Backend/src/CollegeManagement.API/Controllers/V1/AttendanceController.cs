using System.Collections.Generic;
using System.Threading.Tasks;
using Asp.Versioning;
using CollegeManagement.API.DTOs.Attendance.Requests;
using CollegeManagement.API.DTOs.Attendance.Responses;
using CollegeManagement.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CollegeManagement.API.Controllers.V1
{
    /// <summary>
    /// API controller for Attendance module endpoints, handling routing, REST conventions, and service orchestration.
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/attendance")]
    [Authorize]
    [Produces("application/json")]
    public class AttendanceController : ControllerBase
    {
        private readonly IAttendanceService _attendanceService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AttendanceController"/> class.
        /// </summary>
        /// <param name="attendanceService">The attendance service dependency.</param>
        public AttendanceController(IAttendanceService attendanceService)
        {
            _attendanceService = attendanceService;
        }

        /// <summary>
        /// Creates a new student attendance record.
        /// </summary>
        /// <param name="request">The attendance record details.</param>
        /// <returns>The ID of the newly created attendance record.</returns>
        /// <response code="201">Attendance record created successfully.</response>
        /// <response code="400">Invalid validation request details.</response>
        /// <response code="401">Unauthorized access.</response>
        /// <response code="409">Conflict with an existing attendance record.</response>
        /// <response code="500">Internal server error.</response>
        [HttpPost("create")]
        [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateAttendance([FromBody] CreateAttendanceRequest request)
        {
            var id = await _attendanceService.CreateAttendanceAsync(request);
            return CreatedAtAction(
                nameof(GetAttendanceById),
                new
                {
                    version = HttpContext.GetRequestedApiVersion()?.ToString(),
                    attendanceId = id
                },
                id);
        }

        /// <summary>
        /// Creates multiple student attendance records in bulk.
        /// </summary>
        /// <param name="request">The bulk attendance details.</param>
        /// <returns>The number of records successfully created.</returns>
        /// <response code="200">Bulk attendance created successfully.</response>
        /// <response code="400">Invalid bulk attendance request parameters.</response>
        /// <response code="401">Unauthorized access.</response>
        /// <response code="409">Conflict with existing attendance records.</response>
        /// <response code="500">Internal server error.</response>
        [HttpPost("bulk")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateBulkAttendance([FromBody] BulkAttendanceRequest request)
        {
            var affectedRows = await _attendanceService.CreateBulkAttendanceAsync(request);
            return Ok(affectedRows);
        }

        /// <summary>
        /// Updates an existing attendance record.
        /// </summary>
        /// <param name="request">The updated attendance values.</param>
        /// <returns>The number of updated records.</returns>
        /// <response code="200">Attendance updated successfully.</response>
        /// <response code="400">Invalid update configuration values.</response>
        /// <response code="401">Unauthorized access.</response>
        /// <response code="404">Attendance record not found.</response>
        /// <response code="500">Internal server error.</response>
        [HttpPut("update")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateAttendance([FromBody] UpdateAttendanceRequest request)
        {
            var affectedRows = await _attendanceService.UpdateAttendanceAsync(request);
            return Ok(affectedRows);
        }

        /// <summary>
        /// Retrieves a single detailed attendance record by its ID.
        /// </summary>
        /// <param name="attendanceId">The attendance identifier.</param>
        /// <returns>The matching attendance response details.</returns>
        /// <response code="200">Attendance record retrieved successfully.</response>
        /// <response code="401">Unauthorized access.</response>
        /// <response code="404">Attendance record not found.</response>
        /// <response code="500">Internal server error.</response>
        [HttpGet("{attendanceId}")]
        [ProducesResponseType(typeof(AttendanceResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAttendanceById(int attendanceId)
        {
            var response = await _attendanceService.GetAttendanceByIdAsync(attendanceId);
            return Ok(response);
        }

        /// <summary>
        /// Searches and filters attendance records.
        /// </summary>
        /// <param name="request">The search filter criteria.</param>
        /// <returns>A list of matching attendance records.</returns>
        /// <response code="200">Filtered list retrieved successfully.</response>
        /// <response code="400">Invalid query filter parameters.</response>
        /// <response code="401">Unauthorized access.</response>
        /// <response code="500">Internal server error.</response>
        [HttpPost("search")]
        [ProducesResponseType(typeof(IEnumerable<AttendanceListResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAttendances([FromBody] AttendanceSearchRequest request)
        {
            var results = await _attendanceService.GetAttendancesAsync(request);
            return Ok(results);
        }

        /// <summary>
        /// Retrieves students available to mark attendance for the specified search criteria.
        /// </summary>
        /// <param name="request">The search filter criteria.</param>
        /// <returns>A list of matching student attendance details.</returns>
        /// <response code="200">Student attendance list loaded successfully.</response>
        /// <response code="400">Invalid search filter parameters.</response>
        /// <response code="401">Unauthorized access.</response>
        /// <response code="500">Internal server error.</response>
        [HttpPost("students")]
        [ProducesResponseType(typeof(IEnumerable<StudentAttendanceResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetStudentsForAttendance([FromBody] AttendanceSearchRequest request)
        {
            var results = await _attendanceService.GetStudentsForAttendanceAsync(request);
            return Ok(results);
        }

        /// <summary>
        /// Retrieves statistical summary metrics for the specified filters.
        /// </summary>
        /// <param name="request">The search filter criteria.</param>
        /// <returns>Statistical attendance summary metrics.</returns>
        /// <response code="200">Summary loaded successfully.</response>
        /// <response code="400">Invalid summary request parameters.</response>
        /// <response code="401">Unauthorized access.</response>
        /// <response code="500">Internal server error.</response>
        [HttpPost("summary")]
        [ProducesResponseType(typeof(AttendanceSummaryResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAttendanceSummary([FromBody] AttendanceSearchRequest request)
        {
            var summary = await _attendanceService.GetAttendanceSummaryAsync(request);
            return Ok(summary);
        }

        /// <summary>
        /// Retrieves attendance percentages and class counts per student for the specified filters.
        /// </summary>
        /// <param name="request">The search filter criteria.</param>
        /// <returns>A list of student attendance percentages.</returns>
        /// <response code="200">Percentage list loaded successfully.</response>
        /// <response code="400">Invalid percentage query parameters.</response>
        /// <response code="401">Unauthorized access.</response>
        /// <response code="500">Internal server error.</response>
        [HttpPost("percentage")]
        [ProducesResponseType(typeof(IEnumerable<AttendancePercentageResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAttendancePercentage([FromBody] AttendanceSearchRequest request)
        {
            var results = await _attendanceService.GetAttendancePercentageAsync(request);
            return Ok(results);
        }

        /// <summary>
        /// Generates a flat report listing attendance details for the specified filters.
        /// </summary>
        /// <param name="request">The search filter criteria.</param>
        /// <returns>A list of attendance report entries.</returns>
        /// <response code="200">Attendance report generated successfully.</response>
        /// <response code="400">Invalid report query parameters.</response>
        /// <response code="401">Unauthorized access.</response>
        /// <response code="500">Internal server error.</response>
        [HttpPost("report")]
        [ProducesResponseType(typeof(IEnumerable<AttendanceReportResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAttendanceReport([FromBody] AttendanceSearchRequest request)
        {
            var results = await _attendanceService.GetAttendanceReportAsync(request);
            return Ok(results);
        }

        /// <summary>
        /// Changes the active/inactive status of an attendance record.
        /// </summary>
        /// <param name="attendanceId">The unique identifier of the attendance record.</param>
        /// <param name="isActive">The target active status flag.</param>
        /// <returns>The number of affected records.</returns>
        /// <response code="200">Status changed successfully.</response>
        /// <response code="400">Invalid request parameter values.</response>
        /// <response code="401">Unauthorized access.</response>
        /// <response code="404">Attendance record not found.</response>
        /// <response code="500">Internal server error.</response>
        [HttpPatch("{attendanceId}/status")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ChangeStatus(int attendanceId, [FromQuery] bool isActive)
        {
            var affectedRows = await _attendanceService.ChangeAttendanceActiveStatusAsync(attendanceId, isActive);
            return Ok(affectedRows);
        }
    }
}
