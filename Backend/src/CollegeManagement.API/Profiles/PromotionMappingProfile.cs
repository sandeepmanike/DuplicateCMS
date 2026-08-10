using AutoMapper;
using CollegeManagement.API.DTOs.Promotion;
using CollegeManagement.API.Models.Promotion;

namespace CollegeManagement.API.Profiles
{
    public class PromotionMappingProfile : Profile
    {
        public PromotionMappingProfile()
        {
            CreateMap<PromotionHistory, PromotionHistoryDto>()
                .ReverseMap();

            CreateMap<PromotionReport, PromotionReportDto>()
                .ReverseMap();

            CreateMap<SectionAllocation, SectionAllocationDto>()
                .ReverseMap();

            CreateMap<GroupAllocation, GroupAllocationDto>()
                .ReverseMap();
        }
    }
}