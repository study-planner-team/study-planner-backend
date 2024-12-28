using StudyPlannerAPI.Models.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudyPlannerTests.Common.EntityFactories
{
    public static class UserFactory
    {
        public static User CreateUser(int userId, string username, string password, string email, string refreshToken, DateTime? refreshTokenExpiry)
        {
            return new User
            {
                UserId = userId,
                Username = username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                Email = email,
                IsPublic = false,
                RefreshToken = refreshToken,
                RefreshTokenExpiryTime = refreshTokenExpiry
            };
        }
    }
}
