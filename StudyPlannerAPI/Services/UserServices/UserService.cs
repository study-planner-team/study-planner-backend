using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using StudyPlannerAPI.Data;
using StudyPlannerAPI.Models;
using StudyPlannerAPI.Models.DTO;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
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
        public async Task<UserRegistrationDTO> RegisterUser(UserRegistrationDTO userDTO)
        {
            var user = _mapper.Map<User>(userDTO);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return userDTO;
        }

        public async Task<string> LoginUser(UserLoginDTO loginDTO)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Username.Equals(loginDTO.Username));

            if (user != null && BCrypt.Net.BCrypt.Verify(loginDTO.Password, user.PasswordHash))
                return CreateToken(user);

            return null;
        }

        public string CreateToken(User user)
        {

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
            };

            var token = new JwtSecurityToken
            (
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(45),
                notBefore: DateTime.UtcNow,
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"])),
                    SecurityAlgorithms.HmacSha256)
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return tokenString;
        }
    }
}
