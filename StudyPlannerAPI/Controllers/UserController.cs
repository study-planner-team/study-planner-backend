﻿using Azure.Core;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StudyPlannerAPI.Models;
using StudyPlannerAPI.Models.Users;
using StudyPlannerAPI.Services.UserServices;
using System.Security.Claims;

namespace StudyPlannerAPI.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private IValidator<UserRegistrationDTO> _registerValidator;
        private IValidator<UserLoginDTO> _loginValidator;
        private IValidator<UserUpdateDTO> _updateValidator;
        public UserController(IUserService userService, IValidator<UserRegistrationDTO> registerValidator, IValidator<UserLoginDTO> loginValidator, IValidator<UserUpdateDTO> updateValidator)
        {
            _userService = userService;
            _registerValidator = registerValidator;
            _loginValidator = loginValidator;
            _updateValidator = updateValidator;
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
                var (accessToken, refreshToken, user) = await _userService.LoginUser(userDTO);

                if (accessToken is null || refreshToken is null)
                    return NotFound("User not found");

                // Set the access token as an HttpOnly cookie
                Response.Cookies.Append("accessToken", accessToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = false, // Wymaga HTTPS
                    SameSite = SameSiteMode.Unspecified,
                    Expires = DateTime.UtcNow.AddMinutes(25)
                });

                // Set the refresh token as an HttpOnly cookie
                Response.Cookies.Append("refreshToken", refreshToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = false, // Wymaga HTTPS
                    SameSite = SameSiteMode.Unspecified,
                    Expires = DateTime.UtcNow.AddDays(7)
                });

                return Ok(user);
            }

            return BadRequest(validationResult.Errors);
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken()
        {
            // Get refresh token from HttpOnly cookie
            if (!Request.Cookies.TryGetValue("refreshToken", out var refreshToken))
            {
                return Unauthorized("Refresh token not found.");
            }

            Console.WriteLine($"Refresh Token (From the request): {refreshToken}");

            var (newAccessToken, newRefreshToken) = await _userService.RefreshToken(refreshToken);

            if (newAccessToken != null && newRefreshToken != null)
            {
                // Set the access token as an HttpOnly cookie
                Response.Cookies.Append("accessToken", newAccessToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = false, // Wymaga HTTPS
                    SameSite = SameSiteMode.Unspecified,
                    Expires = DateTime.UtcNow.AddMinutes(25)
                });

                // Set new refresh token as an HttpOnly cookie
                Response.Cookies.Append("refreshToken", newRefreshToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = false, // Wymaga HTTPS
                    SameSite = SameSiteMode.Unspecified,
                    Expires = DateTime.UtcNow.AddDays(7)
                });

                return Ok("Token has been refreshed");
            }

            return Unauthorized("Invalid or expired refresh token.");
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            // Get the refresh token from the cookie
            if (!Request.Cookies.TryGetValue("refreshToken", out var refreshToken))
            {
                Console.WriteLine(refreshToken);
                return BadRequest("Refresh token not found.");
            }

            // Call the service to log out the user
            var result = await _userService.LogoutUser(refreshToken);

            if (!result)
            {
                return BadRequest("Logout failed. Invalid refresh token or user not found.");
            }

            // Remove the cookies
            Response.Cookies.Delete("accessToken");
            Response.Cookies.Delete("refreshToken");

            return Ok("Logged out successfully.");
        }

        [HttpPut("{id}")]
        [Authorize]
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

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _userService.DeleteUser(id);

            if (!deleted)
                return NotFound("User not found");

            return NoContent(); 
        }

    }
}
