using StudyPlannerAPI.Models.DTO;
using StudyPlannerAPI.Models;
using AutoMapper;

namespace StudyPlannerAPI.Mapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<UserRegistrationDTO, User>()
               .ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(src => BCrypt.Net.BCrypt.HashPassword(src.Password)));
        }
    }
}
