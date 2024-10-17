using AutoMapper;
using StudyPlannerAPI.Models.StudyPlans;
using StudyPlannerAPI.Models.StudySessions;
using StudyPlannerAPI.Models.Users;

namespace StudyPlannerAPI.Mapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<UserRegistrationDTO, User>()
                .ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(src => BCrypt.Net.BCrypt.HashPassword(src.Password)))
                .ForMember(dest => dest.IsPublic, opt => opt.MapFrom(src => false));

            CreateMap<User, UserResponseDTO>();

            CreateMap<UserUpdateDTO, User>();

            // StudyPlan
            CreateMap<StudyPlanDTO, StudyPlan>();
            CreateMap<StudyPlan, StudyPlanResponseDTO>()
                .ForMember(dest => dest.Owner, opt => opt.MapFrom(src => src.User));

            // StudyTopic
            CreateMap<StudyTopicDTO, StudyTopic>();

            // StudySession
            CreateMap<StudySessionDTO, StudySession>();
        }


    }
}
