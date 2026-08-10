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
            CreateMap<SaveMarkDto, Mark>()
                .ForMember(dest => dest.TotalMarks, opt => opt.MapFrom(src => src.InternalMarks + src.PracticalMarks + src.TheoryMarks))
                .ForMember(dest => dest.IsVerified, opt => opt.MapFrom(_ => false))
                .ForMember(dest => dest.IsPublished, opt => opt.MapFrom(_ => false))
                .ForMember(dest => dest.VerifiedBy, opt => opt.Ignore())
                .ForMember(dest => dest.VerifiedAt, opt => opt.Ignore())
                .ForMember(dest => dest.PublishedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(_ => true))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.MarkId, opt => opt.Ignore());

            CreateMap<UpdateMarkDto, Mark>()
                .ForMember(dest => dest.TotalMarks, opt => opt.MapFrom(src => src.InternalMarks + src.PracticalMarks + src.TheoryMarks))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.MarkId, opt => opt.Ignore())
                .ForMember(dest => dest.Board, opt => opt.Ignore())
                .ForMember(dest => dest.AcademicYearId, opt => opt.Ignore())
                .ForMember(dest => dest.AcademicLevel, opt => opt.Ignore())
                .ForMember(dest => dest.GroupId, opt => opt.Ignore())
                .ForMember(dest => dest.SectionId, opt => opt.Ignore())
                .ForMember(dest => dest.ExaminationId, opt => opt.Ignore())
                .ForMember(dest => dest.SubjectId, opt => opt.Ignore())
                .ForMember(dest => dest.StudentId, opt => opt.Ignore())
                .ForMember(dest => dest.RollNo, opt => opt.Ignore())
                .ForMember(dest => dest.StudentName, opt => opt.Ignore())
                .ForMember(dest => dest.IsVerified, opt => opt.Ignore())
                .ForMember(dest => dest.IsPublished, opt => opt.Ignore())
                .ForMember(dest => dest.VerifiedBy, opt => opt.Ignore())
                .ForMember(dest => dest.VerifiedAt, opt => opt.Ignore())
                .ForMember(dest => dest.PublishedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());

            CreateMap<Mark, MarkResponseDto>();
        }
    }
}