using AutoMapper;
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
        }


    }
}
