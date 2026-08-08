using AutoMapper;
using CollegeManagement.API.DTOs.Promotion;
using CollegeManagement.API.Repositories.Interfaces;
using CollegeManagement.API.Services.Interfaces;

namespace CollegeManagement.API.Services.Implementations
{
    public class PromotionService : IPromotionService
    {
        private readonly IPromotionRepository _promotionRepository;
        private readonly IMapper _mapper;

        public PromotionService(
            IPromotionRepository promotionRepository,
            IMapper mapper)
        {
            _promotionRepository = promotionRepository;
            _mapper = mapper;
        }

        public async Task<List<EligibleStudentDto>> GetEligibleStudentsAsync()
        {
            return await _promotionRepository.GetEligibleStudentsAsync();
        }

        public async Task<PromotionResponseDto> PromoteStudentsAsync(PromotionRequestDto dto)
        {
            if (dto.StudentIds == null || !dto.StudentIds.Any())
                throw new Exception("Please select at least one student.");

            return await _promotionRepository.PromoteStudentsAsync(dto);
        }

        public async Task<PromotionResponseDto> PromoteSingleStudentAsync(int studentId)
        {
            if (studentId <= 0)
                throw new Exception("Invalid Student Id.");

            return await _promotionRepository.PromoteSingleStudentAsync(studentId);
        }

        public async Task<List<PromotionHistoryDto>> GetPromotionHistoryAsync()
        {
            return await _promotionRepository.GetPromotionHistoryAsync();
        }

        public async Task<PromotionResponseDto> RollbackPromotionAsync(RollbackPromotionDto dto)
        {
            return await _promotionRepository.RollbackPromotionAsync(dto);
        }

        public async Task<PromotionReportDto> GetPromotionReportAsync()
        {
            return await _promotionRepository.GetPromotionReportAsync();
        }

        public async Task<bool> UpdateSectionAllocationAsync(SectionAllocationDto dto)
        {
            if (dto.StudentIds == null || !dto.StudentIds.Any())
                throw new Exception("Student list cannot be empty.");

            return await _promotionRepository.UpdateSectionAllocationAsync(dto);
        }

        public async Task<bool> UpdateGroupAllocationAsync(GroupAllocationDto dto)
        {
            if (dto.StudentIds == null || !dto.StudentIds.Any())
                throw new Exception("Student list cannot be empty.");

            return await _promotionRepository.UpdateGroupAllocationAsync(dto);
        }
    }
}