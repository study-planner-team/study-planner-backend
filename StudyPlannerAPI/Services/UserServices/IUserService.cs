using StudyPlannerAPI.Models.DTO;

namespace StudyPlannerAPI.Services.UserServices
{
    public interface IUserService
    {
        public Task<UserRegistrationDTO> RegisterUser(UserRegistrationDTO userDTO);
        public Task<string> LoginUser(UserLoginDTO loginDTO);
    }
}
