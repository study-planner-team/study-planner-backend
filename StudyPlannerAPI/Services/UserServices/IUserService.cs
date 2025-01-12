using StudyPlannerAPI.Models.Users;

namespace StudyPlannerAPI.Services.UserServices
{
    public interface IUserService
    {
        public Task<UserRegistrationDTO> RegisterUser(UserRegistrationDTO userDTO);
        public Task<(string?, string?, UserResponseDTO?)> LoginUser(UserLoginDTO loginDTO);
        public Task<(string?, string?)> RefreshToken(string oldRefreshToken);
        public Task<bool> LogoutUser(string refreshToken);
        public Task<UserResponseDTO?> UpdateUser(int userId, UserUpdateDTO userDTO);
        public Task<bool> DeleteUser(int userId);
        public Task<(string?, string?, UserResponseDTO?)> HandleGoogleUser(string email, string name);
        public Task<User> FindOrCreateUserByGoogleAsync(string email, string name);
        public Task<bool> ChangePassword(int userId, UserPasswordChangeDTO passwordChangeDTO);
    }
}
