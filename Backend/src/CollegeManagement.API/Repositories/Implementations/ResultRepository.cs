using System.Data;
using CollegeManagement.API.Data;
using CollegeManagement.API.DTOs.Result;
using CollegeManagement.API.Models;
using CollegeManagement.API.Repositories.Interfaces;
using Dapper;
using Microsoft.EntityFrameworkCore;

namespace CollegeManagement.API.Repositories.Implementations
{
    /// <summary>
    /// Repository implementation for Result database operations using Dapper and MySQL Stored Procedures.
    /// </summary>
    public class ResultRepository : IResultRepository
    {
        private readonly AppDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResultRepository"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        public ResultRepository(AppDbContext context)
        {
            _context = context;
        }

        private IDbConnection Connection => _context.Database.GetDbConnection();

        /// <summary>
        /// Processes examination results using sp_ProcessResults.
        /// </summary>
        public async Task<bool> ProcessResultsAsync(ProcessResultRequestDto request)
        {
            var affected = await Connection.ExecuteScalarAsync<int>(
                "sp_ProcessResults",
                new
                {
                    p_BoardId = request.BoardId,
                    p_AcademicYearId = request.AcademicYearId,
                    p_AcademicLevelId = request.AcademicLevelId,
                    p_GroupId = request.GroupId,
                    p_ExamId = request.ExamId
                },
                commandType: CommandType.StoredProcedure);

            return affected > 0;
        }

        /// <summary>
        /// Publishes processed examination results using sp_PublishResults.
        /// </summary>
        public async Task<bool> PublishResultsAsync(PublishResultRequestDto request)
        {
            var affected = await Connection.ExecuteScalarAsync<int>(
                "sp_PublishResults",
                new
                {
                    p_BoardId = request.BoardId,
                    p_AcademicYearId = request.AcademicYearId,
                    p_AcademicLevelId = request.AcademicLevelId,
                    p_GroupId = request.GroupId,
                    p_ExamId = request.ExamId,
                    p_PublishDate = request.PublishDate
                },
                commandType: CommandType.StoredProcedure);

            return affected > 0;
        }

        /// <summary>
        /// Retrieves all published examination results using sp_GetResults.
        /// </summary>
        public async Task<IEnumerable<ResultDto>> GetResultsAsync()
        {
            var result = await Connection.QueryAsync<ResultDto>(
                "sp_GetResults",
                commandType: CommandType.StoredProcedure);

            return result.ToList();
        }

        /// <summary>
        /// Retrieves published results of a student using sp_GetStudentResult.
        /// </summary>
        public async Task<StudentResultDto?> GetStudentResultAsync(int studentId)
        {
            var result = await Connection.QueryFirstOrDefaultAsync<StudentResultDto>(
                "sp_GetStudentResult",
                new
                {
                    p_StudentId = studentId
                },
                commandType: CommandType.StoredProcedure);

            return result;
        }

        /// <summary>
        /// Retrieves the rank list using sp_GetRankList.
        /// </summary>
        public async Task<IEnumerable<RankListDto>> GetRankListAsync()
        {
            var result = await Connection.QueryAsync<RankListDto>(
                "sp_GetRankList",
                commandType: CommandType.StoredProcedure);

            return result.ToList();
        }

        /// <summary>
        /// Retrieves failed students using sp_GetFailedStudents.
        /// </summary>
        public async Task<IEnumerable<StudentResultDto>> GetFailedStudentsAsync()
        {
            var result = await Connection.QueryAsync<StudentResultDto>(
                "sp_GetFailedStudents",
                commandType: CommandType.StoredProcedure);

            return result.ToList();
        }

        /// <summary>
        /// Retrieves result statistics using sp_GetResultStatistics.
        /// </summary>
        public async Task<ResultStatisticsDto> GetResultStatisticsAsync()
        {
            var result = await Connection.QueryFirstOrDefaultAsync<ResultStatisticsDto>(
                "sp_GetResultStatistics",
                commandType: CommandType.StoredProcedure);

            return result ?? new ResultStatisticsDto();
        }

        /// <summary>
        /// Retrieves result analysis using sp_GetResultAnalysis.
        /// </summary>
        public async Task<object> GetResultAnalysisAsync()
        {
            var result = await Connection.QueryAsync(
                "sp_GetResultAnalysis",
                commandType: CommandType.StoredProcedure);

            return result.ToList();
        }

        /// <summary>
        /// Downloads the student's result memo using sp_DownloadMemo.
        /// </summary>
        public async Task<byte[]> DownloadMemoAsync(int studentId)
        {
            var result = await Connection.QueryAsync<ResultDto>(
                "sp_DownloadMemo",
                new
                {
                    p_StudentId = studentId
                },
                commandType: CommandType.StoredProcedure);

               // PDF generation logic can be implemented later.
            // Returning byte array placeholder for now.
            return System.Text.Encoding.UTF8.GetBytes(
                System.Text.Json.JsonSerializer.Serialize(result));
        }

        /// <summary>
        /// Creates a revaluation request using sp_RequestRevaluation.
        /// </summary>
        public async Task<bool> RequestRevaluationAsync(RevaluationRequestDto request)
        {
            var affected = await Connection.ExecuteScalarAsync<int>(
                "sp_RequestRevaluation",
                new
                {
                    p_ResultId = request.ResultId,
                    p_StudentId = request.StudentId,
                    p_Reason = request.Reason
                },
                commandType: CommandType.StoredProcedure);

            return affected > 0;
        }

        /// <summary>
        /// Retrieves revaluation status using sp_GetRevaluationStatus.
        /// </summary>
        public async Task<RevaluationStatusDto?> GetRevaluationStatusAsync(int revaluationId)
        {
            var result = await Connection.QueryFirstOrDefaultAsync<RevaluationStatusDto>(
                "sp_GetRevaluationStatus",
                new
                {
                    p_RevaluationId = revaluationId
                },
                commandType: CommandType.StoredProcedure);

            return result;
        }

    }

}
