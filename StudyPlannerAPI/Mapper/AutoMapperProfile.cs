using AutoMapper;
using StudyPlannerAPI.Models.StudyMaterials;
using StudyPlannerAPI.Models.StudyPlans;
using StudyPlannerAPI.Models.StudySessions;
using StudyPlannerAPI.Models.StudyTopics;
using StudyPlannerAPI.Models.Users;

namespace StudyPlannerAPI.Mapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // User
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
            CreateMap<StudyTopic, StudyTopicResponseDTO>()
            .ForMember(dest => dest.StudyMaterials, opt => opt.MapFrom(src => src.StudyMaterials));

            // StudySession
            CreateMap<StudySessionDTO, StudySession>();

            // StudyMaterial
            CreateMap<StudyMaterialDTO, StudyMaterial>();
            CreateMap<StudyMaterial, StudyMaterialResponseDTO>();

        }
    }
}
