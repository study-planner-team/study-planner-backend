﻿namespace StudyPlannerAPI.Models.Users
{
    public class UserRegistrationDTO
    {
        public required string Username { get; set; }
        public required string Password { get; set; }
        public required string Email { get; set; }
    }
}
