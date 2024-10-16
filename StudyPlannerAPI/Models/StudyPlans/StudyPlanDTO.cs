namespace StudyPlannerAPI.Models.StudyPlans
{
    public class StudyPlanDTO
    {
        public required string Title { get; set; }
        public string? Description { get; set; }
        public string? Category { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsPublic { get; set; }
        public bool IsArchived { get; set; }
    }
}