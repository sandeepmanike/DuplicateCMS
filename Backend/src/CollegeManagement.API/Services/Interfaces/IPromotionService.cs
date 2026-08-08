using CollegeManagement.API.DTOs.Promotion;

namespace CollegeManagement.API.Services.Interfaces
{
    public interface IPromotionService
    {
        Task<List<EligibleStudentDto>> GetEligibleStudentsAsync();

        Task<PromotionResponseDto> PromoteStudentsAsync(PromotionRequestDto dto);

        Task<PromotionResponseDto> PromoteSingleStudentAsync(int studentId);

        Task<List<PromotionHistoryDto>> GetPromotionHistoryAsync();

        Task<PromotionResponseDto> RollbackPromotionAsync(RollbackPromotionDto dto);

        Task<PromotionReportDto> GetPromotionReportAsync();

        Task<bool> UpdateSectionAllocationAsync(SectionAllocationDto dto);

        Task<bool> UpdateGroupAllocationAsync(GroupAllocationDto dto);
    }
}