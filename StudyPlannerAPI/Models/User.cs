using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudyPlannerAPI.Models
{
    public class User
    {
        [Key]
        public required int UserId { get; set; }

        public required string Username { get; set; }

        public required string PasswordHash { get; set; } 

        public required string Email { get; set; }
    }
}
