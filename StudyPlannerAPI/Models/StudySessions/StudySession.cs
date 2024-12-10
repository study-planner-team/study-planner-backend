using StudyPlannerAPI.Models.StudyPlans;
using StudyPlannerAPI.Models.StudyTopics;
using StudyPlannerAPI.Models.Users;
using System.ComponentModel.DataAnnotations;

namespace StudyPlannerAPI.Models.StudySessions
{
    public enum StudySessionStatus
    {
        NotStarted,
        InProgress,
        Completed,
        Missed
    }
    public class StudySession
    {
        [Key]
        public int StudySessionId { get; set; }
        public required DateTime Date { get; set; }
        public required double Duration { get; set; }
        public required TimeSpan StartTime { get; set; }
        public required TimeSpan EndTime { get; set; }
        public StudySessionStatus Status { get; set; } = StudySessionStatus.NotStarted;
        public TimeSpan? ActualStartTime { get; set; }
        public TimeSpan? ActualDuration { get; set; }  

        public int StudyPlanId { get; set; }
        public StudyPlan StudyPlan { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public int TopicId { get; set; }
        public StudyTopic StudyTopic { get; set; }
    }
}
