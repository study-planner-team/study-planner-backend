using StudyPlannerAPI.Models.StudyPlans;
using StudyPlannerAPI.Models.StudySessions;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudyPlannerAPI.Models.Users
{
    public class User
    {
        [Key]
        public required int UserId { get; set; }

        public required string Username { get; set; }

        public required string PasswordHash { get; set; }

        public required string Email { get; set; }

        public required bool IsPublic { get; set; }

        public string? RefreshToken { get; set; }

        public DateTime? RefreshTokenExpiryTime { get; set; }

        public ICollection<StudySession> StudySessions { get; set; }
    }
}
