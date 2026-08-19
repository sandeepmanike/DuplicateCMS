using AutoMapper;
using CollegeManagement.API.DTOs.Result;
using CollegeManagement.API.Exceptions;
using CollegeManagement.API.Repositories.Interfaces;
using CollegeManagement.API.Services.Interfaces;

using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace CollegeManagement.API.Services.Implementations
{
    /// <summary>
    /// Service implementation for Result operations, handling validations and DTO mappings.
    /// </summary>
    public class ResultService : IResultService
    {
        private readonly IResultRepository _resultRepository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResultService"/> class.
        /// </summary>
        /// <param name="resultRepository">The result repository dependency.</param>
        /// <param name="mapper">The AutoMapper instance.</param>
        public ResultService(IResultRepository resultRepository, IMapper mapper)
        {
            _resultRepository = resultRepository;
            _mapper = mapper;
        }

        #region Core Result Actions

        /// <summary>
        /// Processes examination results.
        /// </summary>
        public async Task<ProcessResultResponseDto> ProcessResultsAsync(
    ProcessResultRequestDto request)
        {
            await ValidateProcessRequestAsync(request);

            var result = await _resultRepository.ProcessResultsAsync(request);

            if (result == null)
            {
                throw new ValidationException(
                    "Unable to process results.");
            }

            return result;
        }



        /// <summary>
        /// Publishes processed examination results.
        /// </summary>
        public async Task<bool> PublishResultsAsync(PublishResultRequestDto request)
        {
            await ValidatePublishRequestAsync(request);

            var published = await _resultRepository.PublishResultsAsync(request);

            if (!published)
            {
                throw new ValidationException("Unable to publish results.");
            }

            return true;
        }

        /// <summary>
        /// Retrieves all examination results.
        /// </summary>
        public async Task<GetResultsResponseDto> GetResultsAsync(
     GetResultsRequestDto request)
        {
            if (request.BoardId <= 0)
                throw new ArgumentException("Invalid BoardId.");

            if (request.AcademicYearId <= 0)
                throw new ArgumentException("Invalid AcademicYearId.");

            if (request.AcademicLevelId <= 0)
                throw new ArgumentException("Invalid AcademicLevelId.");

            if (request.GroupId <= 0)
                throw new ArgumentException("Invalid GroupId.");

            if (request.ExamId <= 0)
                throw new ArgumentException("Invalid ExamId.");

            if (request.PageNumber <= 0)
                request.PageNumber = 1;

            if (request.PageSize <= 0)
                request.PageSize = 10;

            return await _resultRepository.GetResultsAsync(request);
        }

        /// <summary>
        /// Retrieves a student's published result.
        /// </summary>
        public async Task<StudentResultDto> GetStudentResultAsync(
    int studentId,
    int boardId,
    int academicYearId,
    int academicLevelId,
    int groupId,
    int examId)
        {
            if (studentId <= 0)
                throw new ArgumentException("Invalid StudentId.");

            if (boardId <= 0)
                throw new ArgumentException("Invalid BoardId.");

            if (academicYearId <= 0)
                throw new ArgumentException("Invalid AcademicYearId.");

            if (academicLevelId <= 0)
                throw new ArgumentException("Invalid AcademicLevelId.");

            if (groupId <= 0)
                throw new ArgumentException("Invalid GroupId.");

            if (examId <= 0)
                throw new ArgumentException("Invalid ExamId.");


            return await _resultRepository.GetStudentResultAsync(
                studentId,
                boardId,
                academicYearId,
                academicLevelId,
                groupId,
                examId);
        }

        /// <summary>
        /// Retrieves the rank list.
        /// </summary>
        public async Task<IEnumerable<RankListDto>> GetRankListAsync(
    int boardId,
    int academicYearId,
    int academicLevelId,
    int groupId,
    int examId)
        {
            if (boardId <= 0)
                throw new ArgumentException("Invalid BoardId.");

            if (academicYearId <= 0)
                throw new ArgumentException("Invalid AcademicYearId.");

            if (academicLevelId <= 0)
                throw new ArgumentException("Invalid AcademicLevelId.");

            if (groupId <= 0)
                throw new ArgumentException("Invalid GroupId.");

            if (examId <= 0)
                throw new ArgumentException("Invalid ExamId.");

            return await _resultRepository.GetRankListAsync(
                boardId,
                academicYearId,
                academicLevelId,
                groupId,
                examId);
        }

        /// <summary>
        /// Retrieves all failed students.
        /// </summary>
        public async Task<IEnumerable<StudentResultDto>> GetFailedStudentsAsync()
        {
            var students = await _resultRepository.GetFailedStudentsAsync();

            return _mapper.Map<IEnumerable<StudentResultDto>>(students);
        }

        /// <summary>
        /// Retrieves result statistics.
        /// </summary>
        public async Task<ResultStatisticsDto> GetResultStatisticsAsync()
        {
            var statistics = await _resultRepository.GetResultStatisticsAsync();

            return _mapper.Map<ResultStatisticsDto>(statistics);
        }

        /// <summary>
        /// Retrieves result analysis.
        /// </summary>
        public async Task<ResultAnalysisDto> GetResultAnalysisAsync(
     int boardId,
     int academicYearId,
     int academicLevelId,
     int groupId,
     int examId)
        {
            if (boardId <= 0)
                throw new ValidationException("Board is required.");

            if (academicYearId <= 0)
                throw new ValidationException("Academic Year is required.");

            if (academicLevelId <= 0)
                throw new ValidationException("Academic Level is required.");

            if (groupId <= 0)
                throw new ValidationException("Group is required.");

            if (examId <= 0)
                throw new ValidationException("Exam is required.");

            return await _resultRepository.GetResultAnalysisAsync(
                boardId,
                academicYearId,
                academicLevelId,
                groupId,
                examId);
        }

        /// <summary>
        /// Downloads the student's published result memo.
        /// </summary>
        public async Task<byte[]> DownloadMemoAsync(
            int studentId,
            int boardId,
            int academicYearId,
            int academicLevelId,
            int groupId,
            int examId)
                {
                    if (studentId <= 0)
                    {
                        throw new ValidationException("Student is required.");
                    }

                    if (boardId <= 0)
                    {
                        throw new ValidationException("Board is required.");
                    }

                    if (academicYearId <= 0)
                    {
                        throw new ValidationException("Academic Year is required.");
                    }

                    if (academicLevelId <= 0)
                    {
                        throw new ValidationException("Academic Level is required.");
                    }

                    if (groupId <= 0)
                    {
                        throw new ValidationException("Group is required.");
                    }

                    if (examId <= 0)
                    {
                        throw new ValidationException("Exam is required.");
                    }

                    var results = await _resultRepository.DownloadMemoAsync(
                        studentId,
                        boardId,
                        academicYearId,
                        academicLevelId,
                        groupId,
                        examId);

                    var resultList = results.ToList();

                    if (!resultList.Any())
                    {
                        throw new NotFoundException(
                            $"Published result memo not found for Student ID {studentId}.");
                    }

                    QuestPDF.Settings.License = LicenseType.Community;

                    var firstResult = resultList.First();

                    var document = Document.Create(container =>
                    {
                        container.Page(page =>
                        {
                            page.Size(PageSizes.A4);
                            page.Margin(40);

                            // Header
                            page.Header()
                                .AlignCenter()
                                .Column(column =>
                                {
                                    column.Item()
                                        .Text("COLLEGE MANAGEMENT SYSTEM")
                                        .Bold()
                                        .FontSize(20);

                                    column.Item()
                                        .Text("EXAMINATION RESULT MEMO")
                                        .Bold()
                                        .FontSize(16);
                                });

                            // Content
                            page.Content()
                                .PaddingTop(25)
                                .Column(column =>
                                {
                                    column.Spacing(10);

                                    column.Item()
                                        .Text($"Student ID: {studentId}")
                                        .FontSize(11);

                                    column.Item()
                                        .Text($"Board ID: {boardId}")
                                        .FontSize(11);

                                    column.Item()
                                        .Text($"Academic Year ID: {academicYearId}")
                                        .FontSize(11);

                                    column.Item()
                                        .Text($"Academic Level ID: {academicLevelId}")
                                        .FontSize(11);

                                    column.Item()
                                        .Text($"Group ID: {groupId}")
                                        .FontSize(11);

                                    column.Item()
                                        .Text($"Exam ID: {examId}")
                                        .FontSize(11);

                                    column.Item()
                                        .PaddingTop(15)
                                        .Table(table =>
                                        {
                                            table.ColumnsDefinition(columns =>
                                            {
                                                columns.RelativeColumn(2);
                                                columns.RelativeColumn(1);
                                                columns.RelativeColumn(1);
                                                columns.RelativeColumn(1);
                                                columns.RelativeColumn(1);
                                                columns.RelativeColumn(1);
                                            });

                                            table.Header(header =>
                                            {
                                                header.Cell()
                                                    .Element(HeaderCell)
                                                    .Text("Subject");

                                                header.Cell()
                                                    .Element(HeaderCell)
                                                    .Text("Internal");

                                                header.Cell()
                                                    .Element(HeaderCell)
                                                    .Text("Practical");

                                                header.Cell()
                                                    .Element(HeaderCell)
                                                    .Text("External");

                                                header.Cell()
                                                    .Element(HeaderCell)
                                                    .Text("Total");

                                                header.Cell()
                                                    .Element(HeaderCell)
                                                    .Text("Grade");
                                            });

                                            foreach (var result in resultList)
                                            {
                                                table.Cell()
                                                    .Element(BodyCell)
                                                    .Text(result.SubjectName ?? "");

                                                table.Cell()
                                                    .Element(BodyCell)
                                                    .Text(result.InternalMarks.ToString());

                                                table.Cell()
                                                    .Element(BodyCell)
                                                    .Text(result.PracticalMarks.ToString());

                                                table.Cell()
                                                    .Element(BodyCell)
                                                    .Text(result.ExternalMarks.ToString());

                                                table.Cell()
                                                    .Element(BodyCell)
                                                    .Text(result.TotalMarks.ToString());

                                                table.Cell()
                                                    .Element(BodyCell)
                                                    .Text(result.Grade ?? "");
                                            }
                                        });

                                    column.Item()
                                        .PaddingTop(20)
                                        .Text($"Result Status: {firstResult.ResultStatus}")
                                        .Bold();

                                    if (firstResult.Rank.HasValue)
                                    {
                                        column.Item()
                                            .Text($"Rank: {firstResult.Rank}");
                                    }

                                    if (firstResult.PublishedDate.HasValue)
                                    {
                                        column.Item()
                                            .Text(
                                                $"Published Date: {firstResult.PublishedDate.Value:dd-MM-yyyy}");
                                    }
                                });

                            // Footer
                            page.Footer()
                                .AlignCenter()
                                .Text("Generated by College Management System")
                                .FontSize(9);
                        });
                    });

                    return document.GeneratePdf();

                    static IContainer HeaderCell(IContainer container)
                    {
                        return container
                            .Border(1)
                            .Padding(5)
                            .AlignCenter();
                    }

                    static IContainer BodyCell(IContainer container)
                    {
                        return container
                            .Border(1)
                            .Padding(5);
                    }
                }



        /// <summary>
        /// Creates a revaluation request.
        /// </summary>
        public async Task<bool> RequestRevaluationAsync(RevaluationRequestDto request)
        {
            await ValidateRevaluationRequestAsync(request);

            var requested = await _resultRepository.RequestRevaluationAsync(request);

            if (!requested)
            {
                throw new ValidationException("Unable to submit revaluation request.");
            }

            return true;
        }

        /// <summary>
        /// Retrieves revaluation status.
        /// </summary>
        public async Task<RevaluationStatusDto?> GetRevaluationStatusAsync(int revaluationId)
        {
            if (revaluationId <= 0)
            {
                throw new ValidationException("Invalid revaluation ID.");
            }

            var status = await _resultRepository.GetRevaluationStatusAsync(revaluationId);

            if (status == null)
            {
                throw new NotFoundException($"Revaluation with ID {revaluationId} was not found.");
            }

            return _mapper.Map<RevaluationStatusDto>(status);
        }

        

        public async Task<ResultDashboardDto> GetResultDashboardAsync()
        {
            return await _resultRepository.GetResultDashboardAsync();
        }

       

        public async Task<bool> UpdateResultAsync(
    int resultId,
    UpdateResultRequestDto request)
        {
            if (resultId <= 0)
            {
                throw new ValidationException("Invalid Result ID.");
            }

            if (request == null)
            {
                throw new ValidationException("Request cannot be null.");
            }

            if (request.InternalMarks < 0)
            {
                throw new ValidationException(
                    "Internal marks cannot be negative.");
            }

            if (request.PracticalMarks < 0)
            {
                throw new ValidationException(
                    "Practical marks cannot be negative.");
            }

            if (request.ExternalMarks < 0)
            {
                throw new ValidationException(
                    "External marks cannot be negative.");
            }

            var updated = await _resultRepository.UpdateResultAsync(
                resultId,
                request);

            if (!updated)
            {
                throw new ValidationException(
                    "Result cannot be edited. It may not exist or may already be published.");
            }

            return true;
        }

        public async Task<IEnumerable<DownloadResultsPdfDto>> GetResultsForPdfAsync(
    int boardId,
    int academicYearId,
    int academicLevelId,
    int groupId,
    int examId)
        {
            if (boardId <= 0)
                throw new ArgumentException("Invalid BoardId.");

            if (academicYearId <= 0)
                throw new ArgumentException("Invalid AcademicYearId.");

            if (academicLevelId <= 0)
                throw new ArgumentException("Invalid AcademicLevelId.");

            if (groupId <= 0)
                throw new ArgumentException("Invalid GroupId.");

            if (examId <= 0)
                throw new ArgumentException("Invalid ExamId.");

            return await _resultRepository.GetResultsForPdfAsync(
                boardId,
                academicYearId,
                academicLevelId,
                groupId,
                examId);
        }

        public async Task<IEnumerable<ExportResultDto>> GetResultsForExportAsync(
    int boardId,
    int academicYearId,
    int academicLevelId,
    int groupId,
    int examId)
        {
            if (boardId <= 0)
                throw new ArgumentException("Invalid BoardId.");

            if (academicYearId <= 0)
                throw new ArgumentException("Invalid AcademicYearId.");

            if (academicLevelId <= 0)
                throw new ArgumentException("Invalid AcademicLevelId.");

            if (groupId <= 0)
                throw new ArgumentException("Invalid GroupId.");

            if (examId <= 0)
                throw new ArgumentException("Invalid ExamId.");

            return await _resultRepository.GetResultsForExportAsync(
                boardId,
                academicYearId,
                academicLevelId,
                groupId,
                examId);
        }



        #endregion

        #region Private Validation Helper Methods


        /// <summary>
        /// Validates the process result request.
        /// </summary>
        private static Task ValidateProcessRequestAsync(ProcessResultRequestDto request)
        {
            if (request == null)
            {
                throw new ValidationException("Request cannot be null.");
            }

            if (request.BoardId <= 0)
            {
                throw new ValidationException("Board is required.");
            }

            if (request.AcademicYearId <= 0)
            {
                throw new ValidationException("Academic Year is required.");
            }

            if (request.AcademicLevelId <= 0)
            {
                throw new ValidationException("Academic Level is required.");
            }

            if (request.GroupId <= 0)
            {
                throw new ValidationException("Group is required.");
            }

            if (request.ExamId <= 0)
            {
                throw new ValidationException("Exam is required.");
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Validates the publish result request.
        /// </summary>
        private static Task ValidatePublishRequestAsync(PublishResultRequestDto request)
        {
            if (request == null)
            {
                throw new ValidationException("Request cannot be null.");
            }

            if (request.BoardId <= 0)
            {
                throw new ValidationException("Board is required.");
            }

            if (request.AcademicYearId <= 0)
            {
                throw new ValidationException("Academic Year is required.");
            }

            if (request.AcademicLevelId <= 0)
            {
                throw new ValidationException("Academic Level is required.");
            }

            if (request.GroupId <= 0)
            {
                throw new ValidationException("Group is required.");
            }

            if (request.ExamId <= 0)
            {
                throw new ValidationException("Exam is required.");
            }

            if (request.PublishDate == default)
            {
                throw new ValidationException("Publish Date is required.");
            }

            return Task.CompletedTask;
        }


        /// <summary>
        /// Validates the revaluation request.
        /// </summary>
        private static Task ValidateRevaluationRequestAsync(RevaluationRequestDto request)
        {
            if (request == null)
            {
                throw new ValidationException("Request cannot be null.");
            }

            if (request.ResultId <= 0)
            {
                throw new ValidationException("Result is required.");
            }

            if (request.StudentId <= 0)
            {
                throw new ValidationException("Student is required.");
            }

            if (string.IsNullOrWhiteSpace(request.Reason))
            {
                throw new ValidationException("Reason is required.");
            }

            return Task.CompletedTask;
        }

        

        #endregion
    }
}
