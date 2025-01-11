using StudyPlannerAPI.Models.Badges;

namespace StudyPlannerAPI.Services.BadgeService
{
    public interface IBadgeService
    {
        public Task<List<BadgeResponseDTO>> GetUserBadges(int userId);
        public Task AssignBadgesToUser(int userId);
    }
}
