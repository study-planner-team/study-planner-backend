using AutoMapper;
using StudyPlannerAPI.Models.Quizes.RequestDTOs;
using StudyPlannerAPI.Models.Quizes;
using StudyPlannerAPI.Models.StudyMaterials;
using StudyPlannerAPI.Models.StudyPlans;
using StudyPlannerAPI.Models.StudySessions;
using StudyPlannerAPI.Models.StudyTopics;
using StudyPlannerAPI.Models.Users;
using StudyPlannerAPI.Models.Quizes.ResponseDTOs;
using StudyPlannerAPI.Models.Badges;

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
            CreateMap<StudySession, StudySessionResponseDTO>()
                .ForMember(dest => dest.StudyTopic, opt => opt.MapFrom(src => src.StudyTopic));

            // StudyMaterial
            CreateMap<StudyMaterialDTO, StudyMaterial>();
            CreateMap<StudyMaterial, StudyMaterialResponseDTO>();

            // Quiz
            CreateMap<QuizRequestDTO, Quiz>()
                .ForMember(dest => dest.Questions, opt => opt.MapFrom(src => src.Questions));
            CreateMap<QuestionRequestDTO, Question>()
                .ForMember(dest => dest.Options, opt => opt.MapFrom(src => src.Options));
            CreateMap<QuestionOptionRequestDTO, QuestionOption>();

            CreateMap<Quiz, QuizResponseDTO>()
                .ForMember(dest => dest.Questions, opt => opt.MapFrom(src => src.Questions));
            CreateMap<Question, QuestionResponseDTO>()
                .ForMember(dest => dest.Options, opt => opt.MapFrom(src => src.Options));
            CreateMap<QuestionOption, QuestionOptionResponseDTO>();

            CreateMap<QuizAssignment, QuizAssignmentResponseDTO>()
                .ForMember(dest => dest.Quiz, opt => opt.MapFrom(src => src.Quiz))
                .AfterMap((src, dest, context) =>
                {
                    // If the quiz is not completed, remove isCorrect
                    if (src.State != QuizState.Completed)
                    {
                        foreach (var question in dest.Quiz.Questions)
                        {
                            foreach (var option in question.Options)
                            {
                                option.IsCorrect = null;
                            }
                        }
                    }
                });

            CreateMap<Badge, BadgeResponseDTO>()
            .ForMember(dest => dest.Earned, opt => opt.Ignore());
        }
    }
}
