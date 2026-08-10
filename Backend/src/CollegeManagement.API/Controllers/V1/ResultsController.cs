using Asp.Versioning;
using CollegeManagement.API.DTOs.Result;
using CollegeManagement.API.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CollegeManagement.API.Controllers.V1
{
    /// <summary>
    /// API controller for Result module endpoints, handling routing and REST conventions.
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/results")]
    [Produces("application/json")]
    public class ResultController : ControllerBase
    {
        private readonly IResultService _resultService;
        private readonly ILogger<ResultController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResultController"/> class.
        /// </summary>
        /// <param name="resultService">The result service dependency.</param>
        /// <param name="logger">The controller logger dependency.</param>
        public ResultController(IResultService resultService, ILogger<ResultController> logger)
        {
            _resultService = resultService;
            _logger = logger;
        }

        /// <summary>
        /// Processes examination results.
        /// </summary>
        /// <param name="request">The result processing request.</param>
        /// <returns>Success response.</returns>
        /// <response code="200">Results processed successfully.</response>
        /// <response code="400">Invalid request.</response>
        /// <response code="500">Internal server error.</response>
        [HttpPost("process")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ProcessResults([FromBody] ProcessResultRequestDto request)
        {
            _logger.LogInformation("Processing results for Exam ID: {ExamId}", request.ExamId);

            var processed = await _resultService.ProcessResultsAsync(request);

            if (!processed)
            {
                return BadRequest();
            }

            return Ok("Results processed successfully.");
        }

        /// <summary>
        /// Publishes examination results.
        /// </summary>
        /// <param name="request">The publish result request.</param>
        /// <returns>Success response.</returns>
        /// <response code="200">Results published successfully.</response>
        /// <response code="400">Invalid request.</response>
        /// <response code="500">Internal server error.</response>
        [HttpPost("publish")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PublishResults([FromBody] PublishResultRequestDto request)
        {
            _logger.LogInformation("Publishing results for Exam ID: {ExamId}", request.ExamId);

            var published = await _resultService.PublishResultsAsync(request);

            if (!published)
            {
                return BadRequest();
            }

            return Ok("Results published successfully.");
        }

        /// <summary>
        /// Retrieves all published examination results.
        /// </summary>
        /// <returns>A list of examination results.</returns>
        /// <response code="200">Results retrieved successfully.</response>
        /// <response code="500">Internal server error.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ResultDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<ResultDto>>> GetResults()
        {
            _logger.LogInformation("Retrieving all published examination results.");

            var results = await _resultService.GetResultsAsync();

            return Ok(results);
        }

        /// <summary>
        /// Retrieves the published result for a specific student.
        /// </summary>
        /// <param name="studentId">The unique identifier of the student.</param>
        /// <returns>The student's published result.</returns>
        /// <response code="200">Student result retrieved successfully.</response>
        /// <response code="404">Student result not found.</response>
        /// <response code="500">Internal server error.</response>
        [HttpGet("students/{studentId}")]
        [ProducesResponseType(typeof(StudentResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<StudentResultDto>> GetStudentResult(int studentId)
        {
            _logger.LogInformation("Retrieving result for Student ID: {StudentId}", studentId);

            var result = await _resultService.GetStudentResultAsync(studentId);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        /// <summary>
        /// Retrieves the rank list.
        /// </summary>
        /// <returns>A list of ranked students.</returns>
        /// <response code="200">Rank list retrieved successfully.</response>
        /// <response code="500">Internal server error.</response>
        [HttpGet("rank-list")]
        [ProducesResponseType(typeof(IEnumerable<RankListDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<RankListDto>>> GetRankList()
        {
            _logger.LogInformation("Retrieving rank list.");

            var result = await _resultService.GetRankListAsync();

            return Ok(result);
        }

        /// <summary>
        /// Retrieves the list of failed students.
        /// </summary>
        /// <returns>A list of failed students.</returns>
        /// <response code="200">Failed students retrieved successfully.</response>
        /// <response code="500">Internal server error.</response>
        [HttpGet("failed-students")]
        [ProducesResponseType(typeof(IEnumerable<StudentResultDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<StudentResultDto>>> GetFailedStudents()
        {
            _logger.LogInformation("Retrieving failed students.");

            var result = await _resultService.GetFailedStudentsAsync();

            return Ok(result);
        }

        /// <summary>
        /// Retrieves examination result statistics.
        /// </summary>
        /// <returns>Result statistics.</returns>
        /// <response code="200">Statistics retrieved successfully.</response>
        /// <response code="500">Internal server error.</response>
        [HttpGet("statistics")]
        [ProducesResponseType(typeof(ResultStatisticsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResultStatisticsDto>> GetResultStatistics()
        {
            _logger.LogInformation("Retrieving result statistics.");

            var statistics = await _resultService.GetResultStatisticsAsync();

            return Ok(statistics);
        }

        /// <summary>
        /// Retrieves examination result analysis.
        /// </summary>
        /// <returns>Result analysis.</returns>
        /// <response code="200">Result analysis retrieved successfully.</response>
        /// <response code="500">Internal server error.</response>
        [HttpGet("analysis")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetResultAnalysis()
        {
            _logger.LogInformation("Retrieving result analysis.");

            var analysis = await _resultService.GetResultAnalysisAsync();

            return Ok(analysis);
        }

        /// <summary>
        /// Downloads the published result memo for a student.
        /// </summary>
        /// <param name="studentId">The unique identifier of the student.</param>
        /// <returns>The downloadable result memo.</returns>
        /// <response code="200">Memo downloaded successfully.</response>
        /// <response code="404">Memo not found.</response>
        /// <response code="500">Internal server error.</response>
        [HttpGet("memo/{studentId}")]
        [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DownloadMemo(int studentId)
        {
            _logger.LogInformation("Downloading result memo for Student ID: {StudentId}", studentId);

            var file = await _resultService.DownloadMemoAsync(studentId);

            if (file == null || file.Length == 0)
            {
                return NotFound();
            }

            return File(
                file,
                "application/pdf",
                $"ResultMemo_{studentId}.pdf");
        }

        /// <summary>
        /// Creates a revaluation request.
        /// </summary>
        /// <param name="request">The revaluation request details.</param>
        /// <returns>Success response.</returns>
        /// <response code="200">Revaluation request submitted successfully.</response>
        /// <response code="400">Invalid request.</response>
        /// <response code="500">Internal server error.</response>
        [HttpPost("revaluation")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RequestRevaluation([FromBody] RevaluationRequestDto request)
        {
            _logger.LogInformation(
                "Creating revaluation request for Result ID: {ResultId}",
                request.ResultId);

            var created = await _resultService.RequestRevaluationAsync(request);

            if (!created)
            {
                return BadRequest();
            }

            return Ok("Revaluation request submitted successfully.");
        }

        /// <summary>
        /// Retrieves the status of a revaluation request.
        /// </summary>
        /// <param name="revaluationId">The unique identifier of the revaluation request.</param>
        /// <returns>The current revaluation status.</returns>
        /// <response code="200">Revaluation status retrieved successfully.</response>
        /// <response code="404">Revaluation request not found.</response>
        /// <response code="500">Internal server error.</response>
        [HttpGet("revaluation/{revaluationId}")]
        [ProducesResponseType(typeof(RevaluationStatusDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<RevaluationStatusDto>> GetRevaluationStatus(int revaluationId)
        {
            _logger.LogInformation(
                "Retrieving revaluation status for ID: {RevaluationId}",
                revaluationId);

            var status = await _resultService.GetRevaluationStatusAsync(revaluationId);

            if (status == null)
            {
                return NotFound();
            }

            return Ok(status);
        }
    }
}