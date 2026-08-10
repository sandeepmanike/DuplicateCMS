using System;
using AutoMapper;
using CollegeManagement.API.DTOs.Marks;
using CollegeManagement.API.Models;

namespace CollegeManagement.API.Profiles
{
    public class MarksMappingProfile : Profile
    {
        public MarksMappingProfile()
        {
            CreateMap<Mark, MarkResponseDto>()
                .ForMember(dest => dest.TotalMarks, opt => opt.MapFrom(src => src.TotalMarks > 0 ? src.TotalMarks : (src.InternalMarks + src.PracticalMarks + src.TheoryMarks)))
                .ForMember(dest => dest.IsPass, opt => opt.MapFrom(src => (src.InternalMarks + src.PracticalMarks + src.TheoryMarks) >= src.PassingMarks));

            CreateMap<SaveMarkDto, Mark>()
                .ForMember(dest => dest.TotalMarks, opt => opt.MapFrom(src => src.InternalMarks + src.PracticalMarks + src.TheoryMarks))
                .ForMember(dest => dest.IsVerified, opt => opt.MapFrom(_ => false))
                .ForMember(dest => dest.IsPublished, opt => opt.MapFrom(_ => false))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(_ => true))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.MarkId, opt => opt.Ignore());

            CreateMap<UpdateMarkDto, Mark>()
                .ForMember(dest => dest.TotalMarks, opt => opt.MapFrom(src => src.InternalMarks + src.PracticalMarks + src.TheoryMarks))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.MarkId, opt => opt.Ignore());
        }
    }
}