using AutoMapper;
using CollegeManagement.API.DTOs.Result;
using CollegeManagement.API.Exceptions;
using CollegeManagement.API.Repositories.Interfaces;
using CollegeManagement.API.Services.Interfaces;

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
        public async Task<bool> ProcessResultsAsync(ProcessResultRequestDto request)
        {
            await ValidateProcessRequestAsync(request);

            var processed = await _resultRepository.ProcessResultsAsync(request);

            if (!processed)
            {
                throw new ValidationException("Unable to process results.");
            }

            return true;
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
        public async Task<IEnumerable<ResultDto>> GetResultsAsync()
        {
            var results = await _resultRepository.GetResultsAsync();

            return _mapper.Map<IEnumerable<ResultDto>>(results);
        }

        /// <summary>
        /// Retrieves a student's published result.
        /// </summary>
        public async Task<StudentResultDto?> GetStudentResultAsync(int studentId)
        {
            var result = await _resultRepository.GetStudentResultAsync(studentId);

            if (result == null)
            {
                throw new NotFoundException($"Result for Student ID {studentId} was not found.");
            }

            return _mapper.Map<StudentResultDto>(result);
        }

        

        /// <summary>
        /// Retrieves the rank list.
        /// </summary>
        public async Task<IEnumerable<RankListDto>> GetRankListAsync()
        {
            var rankList = await _resultRepository.GetRankListAsync();

            return _mapper.Map<IEnumerable<RankListDto>>(rankList);
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
        public async Task<object> GetResultAnalysisAsync()
        {
            return await _resultRepository.GetResultAnalysisAsync();
        }

        /// <summary>
        /// Downloads the student's result memo.
        /// </summary>
        public async Task<byte[]> DownloadMemoAsync(int studentId)
        {
            if (studentId <= 0)
            {
                throw new ValidationException("Invalid student ID.");
            }

            return await _resultRepository.DownloadMemoAsync(studentId);
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
