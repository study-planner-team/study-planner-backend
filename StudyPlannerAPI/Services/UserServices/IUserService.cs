using StudyPlannerAPI.Models;
using StudyPlannerAPI.Models.DTO;

namespace StudyPlannerAPI.Services.UserServices
{
    public interface IUserService
    {
        public Task<UserRegistrationDTO> RegisterUser(UserRegistrationDTO userDTO);
        public Task<(string?, string?, User?)> LoginUser(UserLoginDTO loginDTO);
        public Task<(string?, string?)> RefreshToken(string oldRefreshToken);
        public Task<bool> LogoutUser(string refreshToken);
    }
}
