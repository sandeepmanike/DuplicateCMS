using AutoMapper;
using CollegeManagement.API.DTOs.Staff;
using CollegeManagement.API.Models.Staff;

namespace CollegeManagement.API.Profiles
{
    public class StaffMappingProfile : Profile
    {
        public StaffMappingProfile()
        {
            // CreateStaffDto -> Staff Entity
            CreateMap<CreateStaffDto, Staff>()
                .ForMember(dest => dest.StaffType, opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.StaffType) ? "Teaching" : src.StaffType))
                .ForMember(dest => dest.FacultyType, opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.StaffType) ? "Teaching" : src.StaffType))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.Status) ? "Active" : src.Status))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => System.DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(_ => false))
                .ForMember(dest => dest.PhotoPath, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.DesignationRef, opt => opt.Ignore());

            // UpdateStaffDto -> Staff Entity
            CreateMap<UpdateStaffDto, Staff>()
                .ForMember(dest => dest.StaffType, opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.StaffType) ? "Teaching" : src.StaffType))
                .ForMember(dest => dest.FacultyType, opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.StaffType) ? "Teaching" : src.StaffType))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => System.DateTime.UtcNow))
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.EmployeeId, opt => opt.Ignore())
                .ForMember(dest => dest.PhotoPath, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.DesignationRef, opt => opt.Ignore());

            // Staff Entity -> StaffResponseDto
            CreateMap<Staff, StaffResponseDto>();

            // AssignStaffSubjectDto -> StaffSubjectAllocation Entity
            CreateMap<AssignStaffSubjectDto, StaffSubjectAllocation>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => System.DateTime.UtcNow))
                .ForMember(dest => dest.Id, opt => opt.Ignore());

            // UpdateStaffSubjectAllocationDto -> StaffSubjectAllocation Entity
            CreateMap<UpdateStaffSubjectAllocationDto, StaffSubjectAllocation>()
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => System.DateTime.UtcNow))
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.StaffId, opt => opt.Ignore());

            // StaffSubjectAllocation Entity -> StaffSubjectAllocationResponseDto
            CreateMap<StaffSubjectAllocation, StaffSubjectAllocationResponseDto>()
                .ForMember(dest => dest.StaffName, opt => opt.MapFrom(src => src.Staff != null ? $"{src.Staff.FirstName} {src.Staff.LastName}".Trim() : string.Empty))
                .ForMember(dest => dest.FacultyName, opt => opt.MapFrom(src => src.Staff != null ? $"{src.Staff.FirstName} {src.Staff.LastName}".Trim() : string.Empty))
                .ForMember(dest => dest.SubjectName, opt => opt.MapFrom(src => src.Subject != null ? src.Subject.SubjectName : string.Empty))
                .ForMember(dest => dest.SubjectCode, opt => opt.MapFrom(src => src.Subject != null ? src.Subject.SubjectCode : string.Empty))
                .ForMember(dest => dest.BoardName, opt => opt.MapFrom(src => src.Subject != null ? (src.Subject.BoardName ?? string.Empty) : string.Empty))
                .ForMember(dest => dest.GroupName, opt => opt.MapFrom(src => src.Subject != null ? (src.Subject.GroupName ?? string.Empty) : string.Empty))
                .ForMember(dest => dest.AcademicLevelName, opt => opt.MapFrom(src => src.Subject != null ? (src.Subject.AcademicLevelName ?? string.Empty) : string.Empty));
        }
    }
}
