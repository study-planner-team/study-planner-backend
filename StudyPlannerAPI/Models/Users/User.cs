using StudyPlannerAPI.Models.StudyPlans;
using StudyPlannerAPI.Models.StudySessions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudyPlannerAPI.Models.Users
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        public required string Username { get; set; }

        public string? PasswordHash { get; set; } // Empty for Google users

        public required string Email { get; set; }

        public required bool IsPublic { get; set; }

        public bool IsGoogleUser { get; set; } = false;

        public string? RefreshToken { get; set; }

        public DateTime? RefreshTokenExpiryTime { get; set; }

        public ICollection<StudySession> StudySessions { get; set; }
    }
}
