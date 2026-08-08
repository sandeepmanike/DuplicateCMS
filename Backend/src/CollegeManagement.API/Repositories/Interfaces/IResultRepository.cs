using CollegeManagement.API.DTOs.Result;

namespace CollegeManagement.API.Repositories.Interfaces
{
    public interface IResultRepository
    {
        Task<bool> ProcessResultsAsync(ProcessResultRequestDto request);

        Task<bool> PublishResultsAsync(PublishResultRequestDto request);

        Task<IEnumerable<ResultDto>> GetResultsAsync();

        Task<StudentResultDto?> GetStudentResultAsync(int studentId);

        Task<IEnumerable<RankListDto>> GetRankListAsync();

        Task<IEnumerable<StudentResultDto>> GetFailedStudentsAsync();

        Task<ResultStatisticsDto> GetResultStatisticsAsync();

        Task<object> GetResultAnalysisAsync();

        Task<byte[]> DownloadMemoAsync(int studentId);

        Task<bool> RequestRevaluationAsync(RevaluationRequestDto request);

        Task<RevaluationStatusDto?> GetRevaluationStatusAsync(int revaluationId);
    }
}