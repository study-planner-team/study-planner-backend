namespace StudyPlannerAPI.Models.Statistics
{
    public class PrecomputedMetricsDTO
    {
        public int CompletedSessions { get; set; }
        public int MissedSessions { get; set; }
        public int InProgressSessions { get; set; }
        public int ActivePlans { get; set; }
        public int ArchivedPlans { get; set; }
        public int JoinedPlans { get; set; }
        public int AssignedQuizCount { get; set; }
        public int CompletedQuizCount { get; set; }
        public double AverageQuizScore { get; set; }
    }
}
