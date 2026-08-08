using System.Collections.Generic;
using System.Threading.Tasks;
using Asp.Versioning;
using CollegeManagement.API.DTOs.Examination.Requests;
using CollegeManagement.API.DTOs.Examination.Responses;
using CollegeManagement.API.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CollegeManagement.API.Controllers.V1
{
    /// <summary>
    /// Controller for managing examinations, schedules, hall tickets, and invigilator assignments.
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/examinations")]
    [Produces("application/json")]
    public class ExaminationController : ControllerBase
    {
        private readonly IExaminationService _examinationService;
        private readonly ILogger<ExaminationController> _logger;

        public ExaminationController(IExaminationService examinationService, ILogger<ExaminationController> logger)
        {
            _examinationService = examinationService;
            _logger = logger;
        }

        #region Examination Base APIs

        /// <summary>
        /// Creates a new examination entry.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(ExaminationResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ExaminationResponse>> CreateExamination([FromBody] CreateExaminationRequest request)
        {
            _logger.LogInformation("Creating examination: {ExamName}", request.ExamName);
            var result = await _examinationService.CreateExaminationAsync(request);

            return CreatedAtAction(
                nameof(GetExaminationById),
                new
                {
                    version = HttpContext.GetRequestedApiVersion()?.ToString(),
                    examinationId = result.ExaminationId
                },
                result);
        }

        /// <summary>
        /// Retrieves all examinations with optional course filtering.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ExaminationResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<ExaminationResponse>>> GetExaminations([FromQuery] string? courseId)
        {
            _logger.LogInformation("Fetching examinations. Course filter: {CourseId}", courseId);
            var result = await _examinationService.GetExaminationsAsync(courseId);
            return Ok(result);
        }

        /// <summary>
        /// Retrieves examination details by unique identifier.
        /// </summary>
        [HttpGet("{examinationId:int}")]
        [ProducesResponseType(typeof(ExaminationResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ExaminationResponse>> GetExaminationById(int examinationId)
        {
            _logger.LogInformation("Fetching examination ID: {Id}", examinationId);
            var result = await _examinationService.GetExaminationByIdAsync(examinationId);
            if (result == null) return NotFound(new { message = "Examination not found." });
            return Ok(result);
        }

        /// <summary>
        /// Updates existing examination details.
        /// </summary>
        [HttpPut("{examinationId:int}")]
        [ProducesResponseType(typeof(ExaminationResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ExaminationResponse>> UpdateExamination(int examinationId, [FromBody] UpdateExaminationRequest request)
        {
            _logger.LogInformation("Updating examination ID: {Id}", examinationId);
            var result = await _examinationService.UpdateExaminationAsync(examinationId, request);
            if (result == null) return NotFound(new { message = "Examination not found." });
            return Ok(result);
        }

        /// <summary>
        /// Deletes an examination entry.
        /// </summary>
        [HttpDelete("{examinationId:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteExamination(int examinationId)
        {
            _logger.LogInformation("Deleting examination ID: {Id}", examinationId);
            var success = await _examinationService.DeleteExaminationAsync(examinationId);
            if (!success) return NotFound(new { message = "Examination not found." });
            return NoContent();
        }

        /// <summary>
        /// Cancels an existing examination.
        /// </summary>
        [HttpPatch("{examinationId:int}/cancel")]
        [ProducesResponseType(typeof(ExaminationStatusResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ExaminationStatusResponse>> CancelExamination(int examinationId, [FromBody] CancelExaminationRequest request)
        {
            _logger.LogInformation("Cancelling examination ID: {Id}", examinationId);
            var result = await _examinationService.CancelExaminationAsync(examinationId, request);
            if (result == null) return NotFound(new { message = "Examination not found." });
            return Ok(result);
        }

        /// <summary>
        /// Reschedules an examination.
        /// </summary>
        [HttpPatch("{examinationId:int}/reschedule")]
        [ProducesResponseType(typeof(ExaminationStatusResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ExaminationStatusResponse>> RescheduleExamination(int examinationId, [FromBody] RescheduleExaminationRequest request)
        {
            _logger.LogInformation("Rescheduling examination ID: {Id}", examinationId);
            var result = await _examinationService.RescheduleExaminationAsync(examinationId, request);
            if (result == null) return NotFound(new { message = "Examination not found." });
            return Ok(result);
        }

        #endregion

        #region Exam Schedule APIs

        /// <summary>
        /// Creates a new schedule entry for an examination.
        /// </summary>
        [HttpPost("schedules")]
        [ProducesResponseType(typeof(ExamScheduleResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ExamScheduleResponse>> CreateExamSchedule([FromBody] CreateExamScheduleRequest request)
        {
            _logger.LogInformation("Creating schedule for Examination ID: {ExamId}", request.ExaminationId);
            var result = await _examinationService.CreateExamScheduleAsync(request);

            return CreatedAtAction(
                nameof(GetExamScheduleById),
                new
                {
                    version = HttpContext.GetRequestedApiVersion()?.ToString(),
                    examScheduleId = result.ExamScheduleId
                },
                result);
        }

        /// <summary>
        /// Retrieves exam schedules, optionally filtered by examination ID.
        /// </summary>
        [HttpGet("schedules")]
        [ProducesResponseType(typeof(IEnumerable<ExamScheduleResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<ExamScheduleResponse>>> GetExamSchedules([FromQuery] int? examinationId)
        {
            _logger.LogInformation("Fetching exam schedules for Examination ID: {ExaminationId}", examinationId);
            var result = await _examinationService.GetExamSchedulesAsync(examinationId);
            return Ok(result);
        }

        /// <summary>
        /// Retrieves an exam schedule by unique schedule ID.
        /// </summary>
        [HttpGet("schedules/{examScheduleId:int}")]
        [ProducesResponseType(typeof(ExamScheduleResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ExamScheduleResponse>> GetExamScheduleById(int examScheduleId)
        {
            _logger.LogInformation("Fetching exam schedule ID: {Id}", examScheduleId);
            var result = await _examinationService.GetExamScheduleByIdAsync(examScheduleId);
            if (result == null) return NotFound(new { message = "Schedule not found." });
            return Ok(result);
        }

        /// <summary>
        /// Updates an existing exam schedule.
        /// </summary>
        [HttpPut("schedules/{examScheduleId:int}")]
        [ProducesResponseType(typeof(ExamScheduleResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ExamScheduleResponse>> UpdateExamSchedule(int examScheduleId, [FromBody] UpdateExamScheduleRequest request)
        {
            _logger.LogInformation("Updating schedule ID: {Id}", examScheduleId);
            var result = await _examinationService.UpdateExamScheduleAsync(examScheduleId, request);
            if (result == null) return NotFound(new { message = "Schedule not found." });
            return Ok(result);
        }

        /// <summary>
        /// Publishes examination schedules.
        /// </summary>
        [HttpPatch("schedules/publish")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PublishExamSchedules([FromBody] PublishExamScheduleRequest request)
        {
            _logger.LogInformation("Publishing exam schedules.");
            var publishedCount = await _examinationService.PublishExamSchedulesAsync(request);
            return Ok(new { message = "Schedules published successfully.", publishedCount });
        }

        #endregion

        #region Hall Ticket APIs

        /// <summary>
        /// Generates hall tickets for an examination and batch.
        /// </summary>
        [HttpPost("halltickets")]
        [ProducesResponseType(typeof(IEnumerable<HallTicketResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<HallTicketResponse>>> GenerateHallTickets([FromBody] GenerateHallTicketRequest request)
        {
            _logger.LogInformation("Generating hall tickets for Examination ID: {ExamId}", request.ExaminationId);
            var result = await _examinationService.GenerateHallTicketsAsync(request);
            return Ok(result);
        }

        /// <summary>
        /// Downloads a student's hall ticket PDF stream.
        /// </summary>
        [HttpGet("halltickets/{studentId:int}")]
        [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK, "application/pdf")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DownloadHallTicket(int studentId, [FromQuery] int examinationId)
        {
            _logger.LogInformation("Downloading hall ticket. Student ID: {StudentId}, Examination ID: {ExamId}", studentId, examinationId);
            var fileStream = await _examinationService.DownloadHallTicketPdfAsync(studentId, examinationId);
            if (fileStream == null) return NotFound(new { message = "Hall ticket not found." });

            return File(fileStream, "application/pdf", $"HallTicket_{studentId}_{examinationId}.pdf");
        }

        #endregion

        #region Invigilator APIs

        /// <summary>
        /// Assigns invigilators to an examination schedule.
        /// </summary>
        [HttpPost("invigilators")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AssignInvigilators([FromBody] AssignInvigilatorRequest request)
        {
            _logger.LogInformation("Assigning invigilators to Schedule ID: {ScheduleId}", request.ExamScheduleId);
            await _examinationService.AssignInvigilatorsAsync(request);
            return Ok(new { message = "Invigilators assigned successfully." });
        }

        /// <summary>
        /// Retrieves invigilator assignments for an exam schedule.
        /// </summary>
        [HttpGet("invigilators")]
        [ProducesResponseType(typeof(IEnumerable<InvigilatorAssignmentResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<InvigilatorAssignmentResponse>>> GetInvigilators([FromQuery] int examScheduleId)
        {
            _logger.LogInformation("Fetching invigilators for Schedule ID: {ScheduleId}", examScheduleId);
            var result = await _examinationService.GetInvigilatorsAsync(examScheduleId);
            return Ok(result);
        }

        #endregion
    }
}