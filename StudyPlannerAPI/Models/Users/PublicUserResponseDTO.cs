using StudyPlannerAPI.Models.Badges;

namespace StudyPlannerAPI.Models.Users
{
    public class PublicUserResponseDTO
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public List<BadgeResponseDTO> Badges { get; set; } = new List<BadgeResponseDTO>();
    }
}
