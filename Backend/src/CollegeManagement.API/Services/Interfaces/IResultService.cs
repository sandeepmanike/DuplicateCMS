using CollegeManagement.API.DTOs.Result;

namespace CollegeManagement.API.Services.Interfaces
{
    public interface IResultService
    {
       
        Task<ProcessResultResponseDto> ProcessResultsAsync(
    ProcessResultRequestDto request);

        Task<bool> PublishResultsAsync(PublishResultRequestDto request);

        Task<GetResultsResponseDto> GetResultsAsync(
    GetResultsRequestDto request);

        Task<StudentResultDto> GetStudentResultAsync(
    int studentId,
    int boardId,
    int academicYearId,
    int academicLevelId,
    int groupId,
    int examId);

        Task<IEnumerable<RankListDto>> GetRankListAsync(
     int boardId,
     int academicYearId,
     int academicLevelId,
     int groupId,
     int examId);

        Task<IEnumerable<StudentResultDto>> GetFailedStudentsAsync();

        Task<ResultStatisticsDto> GetResultStatisticsAsync();

        Task<ResultAnalysisDto> GetResultAnalysisAsync(
    int boardId,
    int academicYearId,
    int academicLevelId,
    int groupId,
    int examId);

        Task<byte[]> DownloadMemoAsync(
    int studentId,
    int boardId,
    int academicYearId,
    int academicLevelId,
    int groupId,
    int examId);
       

        Task<bool> RequestRevaluationAsync(RevaluationRequestDto request);

        Task<RevaluationStatusDto?> GetRevaluationStatusAsync(int revaluationId);

        Task<bool> UpdateResultAsync(
    int resultId,
    UpdateResultRequestDto request);


       

        Task<ResultDashboardDto> GetResultDashboardAsync();


        Task<IEnumerable<DownloadResultsPdfDto>> GetResultsForPdfAsync(
        int boardId,
        int academicYearId,
        int academicLevelId,
        int groupId,
        int examId);

        Task<IEnumerable<ExportResultDto>> GetResultsForExportAsync(
    int boardId,
    int academicYearId,
    int academicLevelId,
    int groupId,
    int examId);

    }
}