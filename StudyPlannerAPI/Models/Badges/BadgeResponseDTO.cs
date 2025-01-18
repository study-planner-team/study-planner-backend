namespace StudyPlannerAPI.Models.Badges
{
    public class BadgeResponseDTO
    {
        public int BadgeId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string IconPath { get; set; }
        public bool Earned { get; set; }
    }
}
