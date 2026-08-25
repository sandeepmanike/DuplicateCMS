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
        private readonly IExaminationExportService _exportService;
        private readonly ILogger<ExaminationController> _logger;

        public ExaminationController(
            IExaminationService examinationService,
            IExaminationExportService exportService,
            ILogger<ExaminationController> logger)
        {
            _examinationService = examinationService;
            _exportService = exportService;
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
        /// Retrieves all examinations with comprehensive academic and status filtering.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ExaminationResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<ExaminationResponse>>> GetExaminations([FromQuery] ExaminationSearchRequestDto filter)
        {
            _logger.LogInformation("Fetching examinations with filter: BoardId={BoardId}, YearId={YearId}, LevelId={LevelId}, GroupId={GroupId}, Status={Status}",
                filter.BoardId, filter.AcademicYearId, filter.AcademicLevelId, filter.GroupId, filter.Status);
            var result = await _examinationService.GetExaminationsAsync(filter);
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
        /// Retrieves eligible subjects and their current schedule status for an examination.
        /// </summary>
        [HttpGet("{examinationId:int}/eligible-subjects")]
        [ProducesResponseType(typeof(IEnumerable<EligibleSubjectResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<EligibleSubjectResponse>>> GetEligibleSubjects(int examinationId)
        {
            _logger.LogInformation("Fetching eligible subjects for examination ID: {Id}", examinationId);
            var result = await _examinationService.GetEligibleSubjectsAsync(examinationId);
            return Ok(result);
        }

        /// <summary>
        /// Finalizes and publishes the examination schedule (transitions status to SCHEDULED).
        /// </summary>
        [HttpPost("{examinationId:int}/finalize-schedule")]
        [ProducesResponseType(typeof(FinalizeScheduleResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<FinalizeScheduleResponse>> FinalizeSchedule(int examinationId)
        {
            _logger.LogInformation("Finalizing schedule for examination ID: {Id}", examinationId);
            var result = await _examinationService.FinalizeScheduleAsync(examinationId);
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
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ExamScheduleResponse>> UpdateExamSchedule(int examScheduleId, [FromBody] UpdateExamScheduleRequest request)
        {
            _logger.LogInformation("Updating schedule ID: {Id}", examScheduleId);
            var result = await _examinationService.UpdateExamScheduleAsync(examScheduleId, request);
            if (result == null) return NotFound(new { message = "Schedule not found." });
            return Ok(result);
        }

        /// <summary>
        /// Deletes an exam schedule entry.
        /// </summary>
        [HttpDelete("schedules/{examScheduleId:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteExamSchedule(int examScheduleId)
        {
            _logger.LogInformation("Deleting exam schedule ID: {Id}", examScheduleId);
            var success = await _examinationService.DeleteExamScheduleAsync(examScheduleId);
            if (!success) return NotFound(new { message = "Schedule not found." });
            return NoContent();
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

        /// <summary>
        /// Creates a batch or combined objective examination schedule for multiple subjects in a single session slot.
        /// </summary>
        [HttpPost("schedules/batch")]
        [ProducesResponseType(typeof(IEnumerable<ExamScheduleResponse>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<ExamScheduleResponse>>> CreateBatchExamSchedules([FromBody] CreateBatchExamScheduleRequest request)
        {
            _logger.LogInformation("Creating batch/combined schedule for Examination ID: {ExamId}, Subjects: {Count}", request.ExaminationId, request.SubjectIds?.Count);
            var result = await _examinationService.CreateBatchExamSchedulesAsync(request);
            return StatusCode(StatusCodes.Status201Created, result);
        }

        /// <summary>
        /// Retrieves list of available halls/rooms for an examination slot without scheduling conflicts.
        /// </summary>
        [HttpGet("available-halls")]
        [ProducesResponseType(typeof(IEnumerable<AvailableHallDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<AvailableHallDto>>> GetAvailableHalls(
            [FromQuery] DateOnly date,
            [FromQuery] TimeOnly startTime,
            [FromQuery] TimeOnly endTime,
            [FromQuery] int? excludeScheduleId)
        {
            _logger.LogInformation("Fetching available halls for Date: {Date}, Time: {StartTime} - {EndTime}", date, startTime, endTime);
            var result = await _examinationService.GetAvailableHallsAsync(date, startTime, endTime, excludeScheduleId);
            return Ok(result);
        }

        /// <summary>
        /// Retrieves list of available faculty members / invigilators for an examination slot without clashes.
        /// </summary>
        [HttpGet("available-invigilators")]
        [ProducesResponseType(typeof(IEnumerable<AvailableInvigilatorDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<AvailableInvigilatorDto>>> GetAvailableInvigilators(
            [FromQuery] DateOnly date,
            [FromQuery] TimeOnly startTime,
            [FromQuery] TimeOnly endTime,
            [FromQuery] int? excludeScheduleId)
        {
            _logger.LogInformation("Fetching available invigilators for Date: {Date}, Time: {StartTime} - {EndTime}", date, startTime, endTime);
            var result = await _examinationService.GetAvailableInvigilatorsAsync(date, startTime, endTime, excludeScheduleId);
            return Ok(result);
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

        #region Export APIs (PDF, Excel, CSV)

        /// <summary>
        /// Exports filtered examinations as a CSV file.
        /// </summary>
        [HttpGet("export/csv")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ExportExaminationsToCsv([FromQuery] ExaminationSearchRequestDto filter)
        {
            _logger.LogInformation("Exporting filtered examinations to CSV.");
            var exams = await _examinationService.GetExaminationsAsync(filter);
            var fileBytes = await _exportService.GenerateExaminationsCsvAsync(exams);
            var fileName = $"examinations-{System.DateTime.UtcNow:yyyyMMdd-HHmmss}.csv";
            return File(fileBytes, "text/csv", fileName);
        }

        /// <summary>
        /// Exports filtered examinations as an Excel workbook.
        /// </summary>
        [HttpGet("export/excel")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ExportExaminationsToExcel([FromQuery] ExaminationSearchRequestDto filter)
        {
            _logger.LogInformation("Exporting filtered examinations to Excel.");
            var exams = await _examinationService.GetExaminationsAsync(filter);
            var fileBytes = await _exportService.GenerateExaminationsExcelAsync(exams);
            var fileName = $"examinations-{System.DateTime.UtcNow:yyyyMMdd-HHmmss}.xlsx";
            const string excelMime = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            return File(fileBytes, excelMime, fileName);
        }

        /// <summary>
        /// Exports filtered examinations as a PDF report.
        /// </summary>
        [HttpGet("export/pdf")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ExportExaminationsToPdf([FromQuery] ExaminationSearchRequestDto filter)
        {
            _logger.LogInformation("Exporting filtered examinations to PDF.");
            var exams = await _examinationService.GetExaminationsAsync(filter);
            var fileBytes = await _exportService.GenerateExaminationsPdfAsync(exams);
            var fileName = $"examinations-{System.DateTime.UtcNow:yyyyMMdd-HHmmss}.pdf";
            return File(fileBytes, "application/pdf", fileName);
        }

        /// <summary>
        /// Exports a specific examination timetable as a CSV file.
        /// </summary>
        [HttpGet("{examinationId:int}/export/csv")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ExportTimetableToCsv(int examinationId)
        {
            _logger.LogInformation("Exporting timetable to CSV for Examination ID: {ExamId}", examinationId);
            var exam = await _examinationService.GetExaminationByIdAsync(examinationId);
            if (exam == null) return NotFound(new { message = "Examination not found." });

            var schedules = await _examinationService.GetExamSchedulesAsync(examinationId);
            var fileBytes = await _exportService.GenerateTimetableCsvAsync(exam, schedules);
            var fileName = $"timetable-{exam.ExamCode ?? examinationId.ToString()}-{System.DateTime.UtcNow:yyyyMMdd}.csv";
            return File(fileBytes, "text/csv", fileName);
        }

        /// <summary>
        /// Exports a specific examination timetable as an Excel workbook.
        /// </summary>
        [HttpGet("{examinationId:int}/export/excel")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ExportTimetableToExcel(int examinationId)
        {
            _logger.LogInformation("Exporting timetable to Excel for Examination ID: {ExamId}", examinationId);
            var exam = await _examinationService.GetExaminationByIdAsync(examinationId);
            if (exam == null) return NotFound(new { message = "Examination not found." });

            var schedules = await _examinationService.GetExamSchedulesAsync(examinationId);
            var fileBytes = await _exportService.GenerateTimetableExcelAsync(exam, schedules);
            var fileName = $"timetable-{exam.ExamCode ?? examinationId.ToString()}-{System.DateTime.UtcNow:yyyyMMdd}.xlsx";
            const string excelMime = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            return File(fileBytes, excelMime, fileName);
        }

        /// <summary>
        /// Exports a specific examination timetable as a PDF report.
        /// </summary>
        [HttpGet("{examinationId:int}/export/pdf")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ExportTimetableToPdf(int examinationId)
        {
            _logger.LogInformation("Exporting timetable to PDF for Examination ID: {ExamId}", examinationId);
            var exam = await _examinationService.GetExaminationByIdAsync(examinationId);
            if (exam == null) return NotFound(new { message = "Examination not found." });

            var schedules = await _examinationService.GetExamSchedulesAsync(examinationId);
            var fileBytes = await _exportService.GenerateTimetablePdfAsync(exam, schedules);
            var fileName = $"timetable-{exam.ExamCode ?? examinationId.ToString()}-{System.DateTime.UtcNow:yyyyMMdd}.pdf";
            return File(fileBytes, "application/pdf", fileName);
        }

        /// <summary>
        /// Exports all scheduled examinations as a global CSV report.
        /// </summary>
        [HttpGet("schedules/export/csv")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ExportScheduledExamsToCsv([FromQuery] ExaminationSearchRequestDto filter)
        {
            _logger.LogInformation("Exporting scheduled examinations to CSV.");
            filter.Status = "SCHEDULED";
            var exams = await _examinationService.GetExaminationsAsync(filter);
            var fileBytes = await _exportService.GenerateScheduledExamsCsvAsync(exams);
            var fileName = $"scheduled-exams-{System.DateTime.UtcNow:yyyyMMdd-HHmmss}.csv";
            return File(fileBytes, "text/csv", fileName);
        }

        /// <summary>
        /// Exports all scheduled examinations as a global Excel report.
        /// </summary>
        [HttpGet("schedules/export/excel")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ExportScheduledExamsToExcel([FromQuery] ExaminationSearchRequestDto filter)
        {
            _logger.LogInformation("Exporting scheduled examinations to Excel.");
            filter.Status = "SCHEDULED";
            var exams = await _examinationService.GetExaminationsAsync(filter);
            var fileBytes = await _exportService.GenerateScheduledExamsExcelAsync(exams);
            var fileName = $"scheduled-exams-{System.DateTime.UtcNow:yyyyMMdd-HHmmss}.xlsx";
            const string excelMime = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            return File(fileBytes, excelMime, fileName);
        }

        /// <summary>
        /// Exports all scheduled examinations as a global PDF report.
        /// </summary>
        [HttpGet("schedules/export/pdf")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ExportScheduledExamsToPdf([FromQuery] ExaminationSearchRequestDto filter)
        {
            _logger.LogInformation("Exporting scheduled examinations to PDF.");
            filter.Status = "SCHEDULED";
            var exams = await _examinationService.GetExaminationsAsync(filter);
            var fileBytes = await _exportService.GenerateScheduledExamsPdfAsync(exams);
            var fileName = $"scheduled-exams-{System.DateTime.UtcNow:yyyyMMdd-HHmmss}.pdf";
            return File(fileBytes, "application/pdf", fileName);
        }

        #endregion
    }
}