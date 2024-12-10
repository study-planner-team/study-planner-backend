using StudyPlannerAPI.Models.StudyTopics;

namespace StudyPlannerAPI.Models.StudySessions
{
    public class StudySessionDTO
    {
        public required int StudyPlanId { get; set; }
        public required DateTime StartDate { get; set; }
        public required DateTime EndDate { get; set; }
        public required int SessionsPerDay { get; set; }
        public required int SessionLength { get; set; }
        public required DateTime StudyStartTime { get; set; }
        public required DateTime StudyEndTime { get; set; }
        public required List<string> PreferredStudyDays { get; set; }
        public required List<int> TopicIds { get; set; }
        public int UserId { get; set; }
    }
}
