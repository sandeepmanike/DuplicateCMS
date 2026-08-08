using AutoMapper;
using CollegeManagement.API.DTOs.Examination.Requests;
using CollegeManagement.API.DTOs.Examination.Responses;
using CollegeManagement.API.Models;

namespace CollegeManagement.API.Profiles
{
    public class ExaminationMappingProfile : Profile
    {
        public ExaminationMappingProfile()
        {
            #region Request -> Model Mappings

            CreateMap<CreateExaminationRequest, Examination>();
            CreateMap<UpdateExaminationRequest, Examination>();

            CreateMap<CreateExamScheduleRequest, ExamSchedule>()
                .ForMember(dest => dest.ExamTime, opt => opt.MapFrom(src => src.StartTime))
                .ForMember(dest => dest.Hall, opt => opt.MapFrom(src => src.RoomNumber))
                .ForMember(dest => dest.Invigilator, opt => opt.MapFrom(src => src.InvigilatorName));

            CreateMap<UpdateExamScheduleRequest, ExamSchedule>()
                .ForMember(dest => dest.ExamTime, opt => opt.MapFrom(src => src.StartTime))
                .ForMember(dest => dest.Hall, opt => opt.MapFrom(src => src.Venue));

            #endregion

            #region Model -> Response Mappings

            CreateMap<Examination, ExaminationResponse>()
                .ForMember(dest => dest.BoardName, opt => opt.MapFrom(src => src.Board != null ? src.Board.BoardName : string.Empty))
                .ForMember(dest => dest.GroupName, opt => opt.MapFrom(src => src.Group != null ? src.Group.GroupName : string.Empty))
                .ForMember(dest => dest.AcademicYear, opt => opt.MapFrom(src => src.AcademicYear != null ? src.AcademicYear.AcademicYearName : string.Empty))
                .ForMember(dest => dest.AcademicLevel, opt => opt.MapFrom(src => src.AcademicLevel != null ? src.AcademicLevel.ToString() : string.Empty))
                .ForMember(dest => dest.ExamType, opt => opt.MapFrom(src => src.AssessmentType != null ? src.AssessmentType.AssessmentTypeName : string.Empty));

            CreateMap<ExamSchedule, ExamScheduleResponse>()
                .ForMember(dest => dest.SubjectName, opt => opt.MapFrom(src => src.Subject != null ? src.Subject.SubjectName : string.Empty))
                .ForMember(dest => dest.StartTime, opt => opt.MapFrom(src => src.ExamTime.ToString("HH:mm")))
                .ForMember(dest => dest.RoomNumber, opt => opt.MapFrom(src => src.Hall))
                .ForMember(dest => dest.InvigilatorName, opt => opt.MapFrom(src => src.Invigilator));

            CreateMap<HallTicket, HallTicketResponse>()
                .ForMember(dest => dest.StudentName, opt => opt.MapFrom(src => src.Student != null ? src.Student.Email : string.Empty))
                .ForMember(dest => dest.RollNumber, opt => opt.MapFrom(src => src.StudentId.ToString()));

            CreateMap<InvigilatorAssignment, InvigilatorAssignmentResponse>()
                .ForMember(dest => dest.InvigilatorName, opt => opt.MapFrom(src => src.Invigilator != null ? src.Invigilator.Email : string.Empty));

            #endregion
        }
    }
}