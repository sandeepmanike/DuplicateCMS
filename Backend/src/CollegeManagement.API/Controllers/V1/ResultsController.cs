using Asp.Versioning;
using ClosedXML.Excel;
using CollegeManagement.API.DTOs.Result;
using CollegeManagement.API.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using QuestPDF.Fluent;
using QuestPDF.Helpers;


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
        public async Task<IActionResult> ProcessResults(
    [FromBody] ProcessResultRequestDto request)
        {
            _logger.LogInformation(
                "Processing results for Exam: {ExamId}",
                request.ExamId);

            var result = await _resultService.ProcessResultsAsync(request);

            return Ok(result);
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
            _logger.LogInformation( "Publishing results for Exam: {ExamId}", request.ExamId);

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
        [HttpGet]
        public async Task<IActionResult> GetResults(
    [FromQuery] GetResultsRequestDto request)
        {
            var result =
                await _resultService.GetResultsAsync(request);

            return Ok(result);
        }

        /// <summary>
        ///  Retrieves the complete result/memo details for a selected student.
        /// </summary>
        /// <param name="studentId">The unique identifier of the student.</param>
        /// <param name="boardId">The unique identifier of the board.</param>
        /// <param name="academicYearId">The unique identifier of the academic year.</param>
        /// <param name="academicLevelId">The unique identifier of the academic level.</param>
        /// <param name="groupId">The unique identifier of the group.</param>
        /// <param name="examId">The unique identifier of the examination.</param>
        /// <returns>The student's published result.</returns>
        /// <response code="200">Student result retrieved successfully.</response>
        /// <response code="404">Student result not found.</response>
        /// <response code="500">Internal server error.</response>
        [HttpGet("student-result")]
        [ProducesResponseType(typeof(StudentResultDto),StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetStudentResult(
            [FromQuery] int studentId,
            [FromQuery] int boardId,
            [FromQuery] int academicYearId,
            [FromQuery] int academicLevelId,
            [FromQuery] int groupId,
            [FromQuery] int examId)
        {
            _logger.LogInformation(
                "Retrieving student result. " +
                "StudentId: {StudentId}, " +
                "BoardId: {BoardId}, " +
                "AcademicYearId: {AcademicYearId}, " +
                "AcademicLevelId: {AcademicLevelId}, " +
                "GroupId: {GroupId}, " +
                "ExamId: {ExamId}",
                studentId,
                boardId,
                academicYearId,
                academicLevelId,
                groupId,
                examId);


            var result = await _resultService.GetStudentResultAsync(
                studentId,
                boardId,
                academicYearId,
                academicLevelId,
                groupId,
                examId);


            return Ok(result);
        }

        /// <summary>
        /// Retrieves the published rank list for the selected
        /// board, academic year, academic level, group and examination.
        /// </summary>
        [HttpGet("rank-list")]
        [ProducesResponseType(typeof(IEnumerable<RankListDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetRankList(
            [FromQuery] int boardId,
            [FromQuery] int academicYearId,
            [FromQuery] int academicLevelId,
            [FromQuery] int groupId,
            [FromQuery] int examId)
        {
            _logger.LogInformation(
                "Retrieving rank list for BoardId: {BoardId}, " +
                "AcademicYearId: {AcademicYearId}, " +
                "AcademicLevelId: {AcademicLevelId}, " +
                "GroupId: {GroupId}, " +
                "ExamId: {ExamId}",
                boardId,
                academicYearId,
                academicLevelId,
                groupId,
                examId);

            var result = await _resultService.GetRankListAsync(
                boardId,
                academicYearId,
                academicLevelId,
                groupId,
                examId);

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
        /// Retrieves examination result analysis for the selected
        /// board, academic year, academic level, group and examination.
        /// </summary>
        [HttpGet("analysis")]
        [ProducesResponseType(typeof(ResultAnalysisDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetResultAnalysis(
            [FromQuery] int boardId,
            [FromQuery] int academicYearId,
            [FromQuery] int academicLevelId,
            [FromQuery] int groupId,
            [FromQuery] int examId)
        {
            _logger.LogInformation(
                "Retrieving result analysis for BoardId: {BoardId}, " +
                "AcademicYearId: {AcademicYearId}, " +
                "AcademicLevelId: {AcademicLevelId}, " +
                "GroupId: {GroupId}, " +
                "ExamId: {ExamId}",
                boardId,
                academicYearId,
                academicLevelId,
                groupId,
                examId);

            var analysis = await _resultService.GetResultAnalysisAsync(
                boardId,
                academicYearId,
                academicLevelId,
                groupId,
                examId);

            return Ok(analysis);
        }


        /// <summary>
        /// Downloads the published result memo for a student.
        /// </summary>
        /// <param name="studentId">The unique identifier of the student.</param>
        /// <param name="boardId">The unique identifier of the board.</param>
        /// <param name="academicYearId">The unique identifier of the academic year.</param>
        /// <param name="academicLevelId">The unique identifier of the academic level.</param>
        /// <param name="groupId">The unique identifier of the group.</param>
        /// <param name="examId">The unique identifier of the examination.</param>
        /// <returns>The student's result memo.</returns>
        /// <response code="200">Result memo downloaded successfully.</response>
        /// <response code="404">Result memo not found.</response>
        /// <response code="500">Internal server error.</response>
        [HttpGet("students/memo")]
        [Produces("application/pdf")]
        [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DownloadMemo(
    [FromQuery] int studentId,
    [FromQuery] int boardId,
    [FromQuery] int academicYearId,
    [FromQuery] int academicLevelId,
    [FromQuery] int groupId,
    [FromQuery] int examId)
        {
            _logger.LogInformation(
                "Downloading result memo for StudentId: {StudentId}, BoardId: {BoardId}, AcademicYearId: {AcademicYearId}, AcademicLevelId: {AcademicLevelId}, GroupId: {GroupId}, ExamId: {ExamId}",
                studentId,
                boardId,
                academicYearId,
                academicLevelId,
                groupId,
                examId);

            var pdf = await _resultService.DownloadMemoAsync(
                studentId,
                boardId,
                academicYearId,
                academicLevelId,
                groupId,
                examId);

            return File(
                pdf,
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

        [HttpGet("dashboard")]
        public async Task<IActionResult> GetResultDashboard()
        {
            var dashboard = await _resultService.GetResultDashboardAsync();

            return Ok(dashboard);
        }


        


        [HttpPut("{resultId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateResult(
    int resultId,
    [FromBody] UpdateResultRequestDto request)
        {
            _logger.LogInformation(
                "Updating Result ID: {ResultId}",
                resultId);

            var updated = await _resultService.UpdateResultAsync(
                resultId,
                request);

            if (!updated)
            {
                return BadRequest(
                    "Result cannot be updated.");
            }

            return Ok("Result updated successfully.");
        }
        [HttpGet("download-pdf")]
        public async Task<IActionResult> DownloadPdf(
            [FromQuery] int boardId,
            [FromQuery] int academicYearId,
            [FromQuery] int academicLevelId,
            [FromQuery] int groupId,
            [FromQuery] int examId)
        {
            _logger.LogInformation(
                "Downloading results PDF for BoardId: {BoardId}, " +
                "AcademicYearId: {AcademicYearId}, " +
                "AcademicLevelId: {AcademicLevelId}, " +
                "GroupId: {GroupId}, " +
                "ExamId: {ExamId}",
                boardId,
                academicYearId,
                academicLevelId,
                groupId,
                examId);

            var results = (
                await _resultService.GetResultsForPdfAsync(
                    boardId,
                    academicYearId,
                    academicLevelId,
                    groupId,
                    examId)
            ).ToList();

            if (!results.Any())
            {
                return NotFound(
                    "No results found for the selected criteria.");
            }

            var first = results.First();

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4.Landscape());

                    page.Margin(20);

                    page.Header()
                        .AlignCenter()
                        .Text("Student Results")
                        .FontSize(20)
                        .Bold();

                    page.Content()
                        .Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(35);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(1);
                            });

                            // Only ONE Header
                            table.Header(header =>
                            {
                                header.Cell()
                                    .Background(Colors.Grey.Lighten2)
                                    .Padding(5)
                                    .Text("S.No")
                                    .Bold();

                                header.Cell()
                                    .Background(Colors.Grey.Lighten2)
                                    .Padding(5)
                                    .Text("Student")
                                    .Bold();

                                header.Cell()
                                    .Background(Colors.Grey.Lighten2)
                                    .Padding(5)
                                    .Text("Roll No")
                                    .Bold();

                                header.Cell()
                                    .Background(Colors.Grey.Lighten2)
                                    .Padding(5)
                                    .Text("Subject")
                                    .Bold();

                                header.Cell()
                                    .Background(Colors.Grey.Lighten2)
                                    .Padding(5)
                                    .Text("Internal")
                                    .Bold();

                                header.Cell()
                                    .Background(Colors.Grey.Lighten2)
                                    .Padding(5)
                                    .Text("Practical")
                                    .Bold();

                                header.Cell()
                                    .Background(Colors.Grey.Lighten2)
                                    .Padding(5)
                                    .Text("Theory")
                                    .Bold();

                                header.Cell()
                                    .Background(Colors.Grey.Lighten2)
                                    .Padding(5)
                                    .Text("Total")
                                    .Bold();
                            });

                            var serialNumber = 1;

                            foreach (var result in results)
                            {
                                table.Cell()
                                    .Padding(5)
                                    .Text(serialNumber++.ToString());

                                table.Cell()
                                    .Padding(5)
                                    .Text(result.StudentName ?? "");

                                table.Cell()
                                    .Padding(5)
                                    .Text(result.RollNumber ?? "");

                                table.Cell()
                                    .Padding(5)
                                    .Text(result.SubjectName ?? "");

                                table.Cell()
                                    .Padding(5)
                                    .Text(result.InternalMarks.ToString());

                                table.Cell()
                                    .Padding(5)
                                    .Text(result.PracticalMarks.ToString());

                                table.Cell()
                                    .Padding(5)
                                    .Text(result.ExternalMarks.ToString());

                                table.Cell()
                                    .Padding(5)
                                    .Text(
                                        $"{result.TotalMarks}/{result.MaximumMarks}");
                            }
                        });

                    page.Footer()
                        .AlignCenter()
                        .Text(text =>
                        {
                            text.Span("Generated on ");
                            text.Span(
                                DateTime.Now.ToString("dd-MM-yyyy HH:mm"));
                        });
                });
            });

            var pdfBytes = document.GeneratePdf();

            var fileName =
                $"Results_{first.ExamName}_{DateTime.Now:yyyyMMddHHmmss}.pdf";

            return File(
                pdfBytes,
                "application/pdf",
                fileName);
        }


        [HttpGet("export-excel")]
        public async Task<IActionResult> ExportExcel(
    [FromQuery] int boardId,
    [FromQuery] int academicYearId,
    [FromQuery] int academicLevelId,
    [FromQuery] int groupId,
    [FromQuery] int examId)
        {
            _logger.LogInformation(
                "Exporting results to Excel for BoardId: {BoardId}, " +
                "AcademicYearId: {AcademicYearId}, " +
                "AcademicLevelId: {AcademicLevelId}, " +
                "GroupId: {GroupId}, " +
                "ExamId: {ExamId}",
                boardId,
                academicYearId,
                academicLevelId,
                groupId,
                examId);

            var results = (
                await _resultService.GetResultsForExportAsync(
                    boardId,
                    academicYearId,
                    academicLevelId,
                    groupId,
                    examId)
            ).ToList();

            if (!results.Any())
            {
                return NotFound(
                    "No results found for the selected criteria.");
            }

            using var workbook = new XLWorkbook();

            var worksheet = workbook.Worksheets.Add("Results");

            // Title
            worksheet.Cell(1, 1).Value = "Student Results";

            worksheet.Range(1, 1, 1, 14).Merge();

            worksheet.Cell(1, 1).Style.Font.Bold = true;
            worksheet.Cell(1, 1).Style.Font.FontSize = 16;

            // Headers
            var headers = new[]
            {
        "S.No",
        "Student Name",
        "Roll Number",
        "Board",
        "Academic Year",
        "Academic Level",
        "Group",
        "Exam",
        "Subject",
        "Internal Marks",
        "Practical Marks",
        "External Marks",
        "Total Marks",
        "Grade"
    };

            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.Cell(3, i + 1).Value = headers[i];
            }

            // Data
            int row = 4;
            int serialNumber = 1;

            foreach (var result in results)
            {
                worksheet.Cell(row, 1).Value = serialNumber++;
                worksheet.Cell(row, 2).Value = result.StudentName;
                worksheet.Cell(row, 3).Value = result.RollNumber;
                worksheet.Cell(row, 4).Value = result.BoardName;
                worksheet.Cell(row, 5).Value = result.AcademicYearName;
                worksheet.Cell(row, 6).Value = result.AcademicLevel;
                worksheet.Cell(row, 7).Value = result.GroupName;
                worksheet.Cell(row, 8).Value = result.ExamName;
                worksheet.Cell(row, 9).Value = result.SubjectName;

                worksheet.Cell(row, 10).Value =
                    result.InternalMarks;

                worksheet.Cell(row, 11).Value =
                    result.PracticalMarks;

                worksheet.Cell(row, 12).Value =
                    result.ExternalMarks;

                worksheet.Cell(row, 13).Value =
                    result.TotalMarks;

                worksheet.Cell(row, 14).Value =
                    result.Grade;

                row++;
            }

            // Format header
            var headerRange = worksheet.Range(
                3,
                1,
                3,
                headers.Length);

            headerRange.Style.Font.Bold = true;

            // Auto-fit columns
            worksheet.Columns().AdjustToContents();

            // Freeze header
            worksheet.SheetView.FreezeRows(3);

            using var stream = new MemoryStream();

            workbook.SaveAs(stream);

            var fileBytes = stream.ToArray();

            var fileName =
                $"Results_{results.First().ExamName}_{DateTime.Now:yyyyMMddHHmmss}.xlsx";

            return File(
                fileBytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileName);
        }

    }
}