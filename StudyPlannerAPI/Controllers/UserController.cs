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
        private IValidator<UserRegistrationDTO> _registerValidator;
        private IValidator<UserLoginDTO> _loginValidator;
        public UserController(IUserService userService, IValidator<UserRegistrationDTO> registerValidator, IValidator<UserLoginDTO> loginValidator)
        {
            _userService = userService;
            _registerValidator = registerValidator;
            _loginValidator = loginValidator;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegistrationDTO userDTO)
        {
            var validationResult = await _registerValidator.ValidateAsync(userDTO);

            if (validationResult.IsValid)
            {
                await _userService.RegisterUser(userDTO);

                return Ok("Konto zostało pomyślnie utworzone.");
            }

            return BadRequest(validationResult.Errors);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDTO userDTO)
        {
            var validationResult = await _loginValidator.ValidateAsync(userDTO);

            if (validationResult.IsValid)
            {
                var token = await _userService.LoginUser(userDTO);

                if (token is null)
                    return NotFound("User not found");

                return Ok(token);
            }

            return BadRequest(validationResult.Errors);
        }
    }
}
