using StudyPlannerAPI.Models.StudyPlans;
using StudyPlannerAPI.Models.Users;
using System.ComponentModel.DataAnnotations;

namespace StudyPlannerAPI.Models.StudySessions
{
    public class StudySession
    {
        [Key]
        public int StudySessionId { get; set; }
        public required DateTime Date { get; set; }
        public required double Duration { get; set; }
        public required string TopicTitle { get; set; }
        public required TimeSpan StartTime { get; set; }
        public required TimeSpan EndTime { get; set; }
        public int StudyPlanId { get; set; }
        public StudyPlan StudyPlan { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }
    }
}
