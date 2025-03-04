﻿using AutoMapper;
using AutoMapper.QueryableExtensions;
using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using StudyPlannerAPI.Data;
using StudyPlannerAPI.Models.Badges;
using StudyPlannerAPI.Models.StudyTopics;
using StudyPlannerAPI.Models.Users;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace StudyPlannerAPI.Services.UserServices
{
    public class UserService : IUserService
    {
        private AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public UserService(AppDbContext context, IMapper mapper, IConfiguration configuration)
        {
            _context = context;
            _mapper = mapper;
            _configuration = configuration;
        }
        public async Task<(string?, string?, UserResponseDTO?)> RegisterUser(UserRegistrationDTO userDTO)
        {
            var normalizedEmail = userDTO.Email.ToLower();
            var normalizedUsername = userDTO.Username.ToLower();

            var userExists = await _context.Users.AnyAsync(u =>
                u.Email.ToLower() == normalizedEmail ||
                u.Username.ToLower() == normalizedUsername);

            if (userExists)
            {
                return (null, null, null);
            }

            var user = _mapper.Map<User>(userDTO);
            user.Email = normalizedEmail;
            user.Username = userDTO.Username;

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Generate tokens
            var accessToken = CreateToken(user, 25); // Replace with your token creation logic
            var refreshToken = GenerateRefreshToken();

            // Save refresh token to the user
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _context.SaveChangesAsync();

            var userResponse = _mapper.Map<UserResponseDTO>(user);
            return (accessToken, refreshToken, userResponse);
        }


        public async Task<(string?,string?, UserResponseDTO?)> LoginUser(UserLoginDTO loginDTO)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == loginDTO.Username);

            if (user != null && !string.Equals(user.Username, loginDTO.Username, StringComparison.Ordinal))
            {
                user = null;
            }

            if (user != null && BCrypt.Net.BCrypt.Verify(loginDTO.Password, user.PasswordHash))
            {
                var accessToken = CreateToken(user, 25);
                var refreshToken = GenerateRefreshToken();

                user.RefreshToken = refreshToken;
                user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
                await _context.SaveChangesAsync();

                var userResponse = _mapper.Map<UserResponseDTO>(user);

                return (accessToken, refreshToken, userResponse);
            }

            return (null,null, null);
        }

        public async Task<(string?, string?)> RefreshToken(string oldRefreshToken)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.RefreshToken == oldRefreshToken);

            Console.WriteLine($"Incoming Refresh Token: {oldRefreshToken}");
            Console.WriteLine($"Stored Refresh Token: {user?.RefreshToken}");

            if (user == null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                return (null, null);
            }

            var newAccessToken = CreateToken(user, 25);
            var newRefreshToken = GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _context.SaveChangesAsync();

            Console.WriteLine($"Generated Refresh Token: {newRefreshToken}");

            return (newAccessToken, newRefreshToken);
        }
        public async Task<bool> LogoutUser(string refreshToken)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);

            if (user == null)
            {
                return false;
            }

            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = null;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<UserResponseDTO?> UpdateUser(int userId, UserUpdateDTO userDTO)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
                return null;

            user.Username = userDTO.Username;
            user.Email = userDTO.Email;
            user.IsPublic = userDTO.IsPublic;

            await _context.SaveChangesAsync();

            return _mapper.Map<UserResponseDTO>(user);
        }


        public async Task<bool> DeleteUser(int userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
                return false;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return true;
        }

        private string CreateToken(User user, int expiration)
        {

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()) //userId
            };

            var token = new JwtSecurityToken
            (
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiration),
                notBefore: DateTime.UtcNow,
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"])),
                    SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];

            using (var numberGenerator = RandomNumberGenerator.Create())
            {
                numberGenerator.GetBytes(randomNumber);
            }

            return Convert.ToBase64String(randomNumber);
        }

        public async Task<(string?, string?, UserResponseDTO?)> HandleGoogleUser(string email, string name)
        {
            var user = await FindOrCreateUserByGoogleAsync(email, name);

            var accessToken = CreateToken(user, 25);
            var refreshToken = GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _context.SaveChangesAsync();

            var userResponse = _mapper.Map<UserResponseDTO>(user);

            return (accessToken, refreshToken, userResponse);

        }

        public async Task<User> FindOrCreateUserByGoogleAsync(string email, string name)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
            {
                user = new User
                {
                    Username = name,
                    Email = email,
                    IsPublic = true,
                    IsGoogleUser = true
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();
            }

            return user;
        }

        public async Task<bool> ChangePassword(int userId, UserPasswordChangeDTO passwordChangeDTO)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null || !BCrypt.Net.BCrypt.Verify(passwordChangeDTO.OldPassword, user.PasswordHash))
            {
                return false;
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(passwordChangeDTO.NewPassword);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<PublicUserResponseDTO>> GetPublicUsersWithBadges(int currentUserId)
        {
            var usersWithBadges = await _context.Users
                .Where(u => u.IsPublic && u.UserId != currentUserId) // Exclude the current user
                .Select(u => new PublicUserResponseDTO
                {
                    UserId = u.UserId,
                    Username = u.Username,
                    Badges = _context.UserBadges
                        .Where(ub => ub.UserId == u.UserId)
                        .Select(ub => new BadgeResponseDTO
                        {
                            BadgeId = ub.Badge.BadgeId,
                            Title = ub.Badge.Title,
                            Description = ub.Badge.Description,
                            IconPath = ub.Badge.IconPath,
                            Earned = true // All badges here are earned
                        })
                        .ToList()
                })
                .ToListAsync();

            return usersWithBadges;
        }
    }
}
