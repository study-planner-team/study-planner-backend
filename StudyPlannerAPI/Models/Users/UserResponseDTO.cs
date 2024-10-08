namespace StudyPlannerAPI.Models.Users
{
    public class UserResponseDTO
    {
        public int UserId { get; set; }
        public required string Username { get; set; }
        public required string Email { get; set; }
        public bool IsPublic { get; set; }
    }
}
