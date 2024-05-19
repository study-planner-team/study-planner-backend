using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StudyPlannerAPI.Models.DTO;
using StudyPlannerAPI.Services.UserServices;

namespace StudyPlannerAPI.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegistrationDTO userDTO)
        {
           await _userService.RegisterUser(userDTO);

           return Ok("Account was successfully created");
        }
    }
}
