using FluentValidation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StudyPlannerAPI.Models;
using StudyPlannerAPI.Models.Users;
using StudyPlannerAPI.Services.UserServices;
using System.Security.Claims;

using Google.Apis.Auth;
using StudyPlannerAPI.Services.BadgeService;
using StudyPlannerAPI.Models.Badges;


namespace StudyPlannerAPI.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IBadgeService _badgeService;
        private IValidator<UserRegistrationDTO> _registerValidator;
        private IValidator<UserLoginDTO> _loginValidator;
        private IValidator<UserUpdateDTO> _updateValidator;
        private IValidator<UserPasswordChangeDTO> _changePasswordValidator;
        public UserController(IUserService userService, IValidator<UserRegistrationDTO> registerValidator, IValidator<UserLoginDTO> loginValidator, IValidator<UserUpdateDTO> updateValidator, IValidator<UserPasswordChangeDTO> changePasswordValidator, IBadgeService badgeService)
        {
            _userService = userService;
            _badgeService = badgeService;
            _registerValidator = registerValidator;
            _loginValidator = loginValidator;
            _updateValidator = updateValidator;
            _changePasswordValidator = changePasswordValidator;
        }

        /// <summary>
        /// Rejestruje nowego użytkownika
        /// </summary>
        /// <response code="200">Zwraca dane nowo zarejestrowanego użytkownika</response>
        /// <response code="400">Jeżeli email lub nazwa użytkownika już istnieją</response>
        [HttpPost("register")]
        [ProducesResponseType(typeof(UserResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] UserRegistrationDTO userDTO)
        {
            var validationResult = await _registerValidator.ValidateAsync(userDTO);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var (accessToken, refreshToken, userResponse) = await _userService.RegisterUser(userDTO);

            if (userResponse == null)
            {
                return BadRequest("Email or username already exists.");
            }

            Response.Cookies.Append("accessToken", accessToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddMinutes(25)
            });

            Response.Cookies.Append("refreshToken", refreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddDays(7)
            });

            return Ok(userResponse);
        }

        /// <summary>
        /// Loguje użytkownika
        /// </summary>
        /// <response code="200">Jeżeli logowanie się powiodło</response>
        /// <response code="400">Jeżeli dane logowania są nieprawidłowe</response>
        /// <response code="404">Jeżeli użytkownik nie istnieje</response>
        [HttpPost("login")]
        [ProducesResponseType(typeof(UserResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Login([FromBody] UserLoginDTO userDTO)
        {
            var validationResult = await _loginValidator.ValidateAsync(userDTO);

            if (validationResult.IsValid)
            {
                var (accessToken, refreshToken, user) = await _userService.LoginUser(userDTO);

                if (accessToken is null || refreshToken is null)
                    return NotFound("User not found");

                Response.Cookies.Append("accessToken", accessToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Expires = DateTime.UtcNow.AddMinutes(25)
                });

                Response.Cookies.Append("refreshToken", refreshToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Expires = DateTime.UtcNow.AddDays(7)
                });

                return Ok(user);
            }

            return BadRequest(validationResult.Errors);
        }

        /// <summary>
        /// Odświeża token dostępu
        /// </summary>
        /// <response code="200">Jeżeli token został odświeżony</response>
        /// <response code="401">Jeżeli refresh token jest nieprawidłowy lub wygasł</response>
        [HttpPost("refresh-token")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> RefreshToken()
        {
            if (!Request.Cookies.TryGetValue("refreshToken", out var refreshToken))
            {
                return Unauthorized("Refresh token not found.");
            }

            var (newAccessToken, newRefreshToken) = await _userService.RefreshToken(refreshToken);

            if (newAccessToken != null && newRefreshToken != null)
            {
                Response.Cookies.Append("accessToken", newAccessToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Expires = DateTime.UtcNow.AddMinutes(25)
                });

                Response.Cookies.Append("refreshToken", newRefreshToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Expires = DateTime.UtcNow.AddDays(7)
                });

                return Ok("Token has been refreshed");
            }

            return Unauthorized("Invalid or expired refresh token.");
        }

        /// <summary>
        /// Wylogowuje użytkownika
        /// </summary>
        /// <response code="200">Jeżeli użytkownik został pomyślnie wylogowany</response>
        /// <response code="400">Jeżeli refresh token nie został znaleziony lub jest nieprawidłowy</response>
        [HttpPost("logout")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Logout()
        {
            if (!Request.Cookies.TryGetValue("refreshToken", out var refreshToken))
            {
                return BadRequest("Refresh token not found.");
            }

            var result = await _userService.LogoutUser(refreshToken);

            if (!result)
            {
                return BadRequest("Logout failed. Invalid refresh token or user not found.");
            }

            Response.Cookies.Delete("accessToken");
            Response.Cookies.Delete("refreshToken");

            return Ok("Logged out successfully.");
        }

        /// <summary>
        /// Aktualizuje dane użytkownika
        /// </summary>
        /// <response code="200">Zwraca zaktualizowane dane użytkownika</response>
        /// <response code="400">Jeżeli dane wejściowe są nieprawidłowe</response>
        /// <response code="404">Jeżeli użytkownik nie istnieje</response>
        [HttpPut("{id}")]
        [Authorize]
        [ProducesResponseType(typeof(UserResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] UserUpdateDTO userUpdateDTO)
        {
            var validationResult = await _updateValidator.ValidateAsync(userUpdateDTO);

            if (validationResult.IsValid)
            {
                var updatedUser = await _userService.UpdateUser(id, userUpdateDTO);

                if (updatedUser == null)
                    return NotFound("User not found");

                return Ok(updatedUser);
            }

            return BadRequest(validationResult.Errors);
        }

        /// <summary>
        /// Usuwa użytkownika
        /// </summary>
        /// <response code="204">Jeżeli użytkownik został usunięty</response>
        /// <response code="404">Jeżeli użytkownik nie istnieje</response>
        [HttpDelete("{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _userService.DeleteUser(id);

            if (!deleted)
                return NotFound("User not found");

            return NoContent();
        }

        /// <summary>
        /// Pobiera odznaki użytkownika
        /// </summary>
        /// <response code="200">Zwraca listę odznak użytkownika</response>
        [HttpGet("{id}/badges")]
        [Authorize]
        [ProducesResponseType(typeof(IEnumerable<BadgeResponseDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUserBadges(int id)
        {
            await _badgeService.AssignBadgesToUser(id);

            var badges = await _badgeService.GetUserBadges(id);
            return Ok(badges);
        }

        /// <summary>
        /// Pobiera dane o publicznych profilach użytkowników
        /// </summary>
        /// <response code="200">Zwraca listę profilów użytkowników</response>
        [HttpGet("profiles/public")]
        [ProducesResponseType(typeof(IEnumerable<PublicUserResponseDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPublicProfiles()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var publicProfiles = await _userService.GetPublicUsersWithBadges(userId);
            return Ok(publicProfiles);
        }

        /// <summary>
        /// Zmienia hasło użytkownika
        /// </summary>
        /// <response code="200">Jeżeli hasło zostało zmienione</response>
        /// <response code="400">Jeżeli stare hasło jest nieprawidłowe</response>
        [HttpPost("{id}/change-password")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ChangePassword(int id, [FromBody] UserPasswordChangeDTO passwordChangeDTO)
        {
            var validationResult = await _changePasswordValidator.ValidateAsync(passwordChangeDTO);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var result = await _userService.ChangePassword(id, passwordChangeDTO);

            if (!result)
            {
                return BadRequest("Incorrect old password or other error occurred.");
            }

            return Ok("Password changed successfully.");
        }

        /// <summary>
        /// Autoryzuje użytkownika za pomocą tokenu JWT Google
        /// </summary>
        /// <response code="200">Zwraca dane użytkownika oraz ustawia tokeny dostępu i odświeżania</response>
        /// <response code="400">Jeżeli autoryzacja z Google nie powiedzie się</response>
        [HttpPost("exchange-google-code")]
        [ProducesResponseType(typeof(UserResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ExchangeGoogleCode([FromBody] string jwtToken)
        {
            try
            {
                // Verify the JWT token using Google's public keys
                var payload = await GoogleJsonWebSignature.ValidateAsync(jwtToken);

                // Extract the user's email and name from the payload
                var email = payload.Email;
                var name = payload.Name;

                // Authenticate or create the user in the database
                var (accessToken, refreshToken, user) = await _userService.HandleGoogleUser(email, name);

                Response.Cookies.Append("accessToken", accessToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true, // Wymaga HTTPS
                    SameSite = SameSiteMode.None,
                    Expires = DateTime.UtcNow.AddMinutes(25)
                });

                Response.Cookies.Append("refreshToken", refreshToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true, // Wymaga HTTPS
                    SameSite = SameSiteMode.None,
                    Expires = DateTime.UtcNow.AddDays(7)
                });

                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest("Failed to authenticate with Google: " + ex.Message);
            }
        }

    }
}