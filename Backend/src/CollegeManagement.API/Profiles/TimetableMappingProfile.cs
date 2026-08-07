using AutoMapper;
using CollegeManagement.API.DTOs.Timetable;
using CollegeManagement.API.Models.Timetable;

namespace CollegeManagement.API.Profiles
{
    public class TimetableMappingProfile : Profile
    {
        public TimetableMappingProfile()
        {
            // Period mappings
            CreateMap<CreatePeriodDto, Period>();
            CreateMap<UpdatePeriodDto, Period>();
            CreateMap<Period, PeriodResponseDto>();

            // Room mappings
            CreateMap<CreateRoomDto, Room>();
            CreateMap<UpdateRoomDto, Room>();
            CreateMap<Room, RoomResponseDto>();
        }
    }
}
