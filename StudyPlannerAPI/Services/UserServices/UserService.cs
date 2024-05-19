using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using StudyPlannerAPI.Data;
using StudyPlannerAPI.Models;
using StudyPlannerAPI.Models.DTO;

namespace StudyPlannerAPI.Services.UserServices
{
    public class UserService : IUserService
    {
        private AppDbContext _context;
        private readonly IMapper _mapper;

        public UserService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<UserRegistrationDTO> RegisterUser(UserRegistrationDTO userDTO)
        {
            var user = _mapper.Map<User>(userDTO);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return userDTO;
        }
    }
}
