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
        private IValidator<UserRegistrationDTO> _validator;
        public UserController(IUserService userService, IValidator<UserRegistrationDTO> validator)
        {
            _userService = userService;
            _validator = validator;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegistrationDTO userDTO)
        {
            var validationResult = await _validator.ValidateAsync(userDTO);

            if (validationResult.IsValid)
            {
                await _userService.RegisterUser(userDTO);

                return Ok("Konto zostało pomyślnie utworzone");
            }

            return BadRequest(validationResult.Errors);
        }
    }
}
