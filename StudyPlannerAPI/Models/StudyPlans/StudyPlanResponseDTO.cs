using StudyPlannerAPI.Models.Users;

namespace StudyPlannerAPI.Models.StudyPlans
{
    public class StudyPlanResponseDTO
    {
        public int StudyPlanId { get; set; }
        public required string Title { get; set; }
        public string? Description { get; set; }
        public string? Category { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsPublic { get; set; }
        public bool IsArchived { get; set; }
        public UserResponseDTO Owner { get; set; }
        public int Progress { get; set; }
    }
}