using StudyPlannerAPI.Models.StudyTopics;

namespace StudyPlannerAPI.Models.StudySessions
{
    public class StudySessionResponseDTO
    {
        public int StudySessionId { get; set; }
        public DateTime Date { get; set; }
        public double Duration { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public StudySessionStatus Status { get; set; }
        public TimeSpan? ActualDuration { get; set; }
        public int StudyPlanId { get; set; }
        public int TopicId { get; set; }
        public StudyTopicResponseDTO StudyTopic { get; set; }
    }
}
