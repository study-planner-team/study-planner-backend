using StudyPlannerAPI.Models.StudyPlans;
using StudyPlannerAPI.Models.StudySessions;

namespace StudyPlannerAPI.Models.Statistics
{
    public class AggregatedStatisticsDTO
    {
        public List<SessionsByDayDTO> SessionsByDay { get; set; }
        public List<SessionsByDayDTO> SessionsMissedByDay { get; set; }
        public List<TimeDistributionDTO> TimeDistribution { get; set; }
        public List<DurationTrendsDTO> DurationTrends { get; set; }
        public List<ProgressTowardGoalsDTO> ProgressTowardGoals { get; set; }
        public List<PreferredStudyTimesDTO> PreferredStudyTimes { get; set; }
        public List<UpcomingSessionsDTO> UpcomingSessions { get; set; }
        public List<TimeDistributionByPlanDTO> TimeDistributionByPlan { get; set; }

    }

    public class SessionsByDayDTO
    {
        public DateTime Date { get; set; }
        public int Count { get; set; }
    }

    public class TimeDistributionDTO
    {
        public string TopicName { get; set; }
        public double TotalTime { get; set; } // Hours
    }

    public class DurationTrendsDTO
    {
        public DateTime Date { get; set; }
        public double TotalActualDuration { get; set; } // Minutes
    }

    public class ProgressTowardGoalsDTO
    {
        public string PlanName { get; set; }
        public double CompletionPercentage { get; set; }
    }

    public class PreferredStudyTimesDTO
    {
        public string Hour { get; set; } // "HH:mm"
        public int Count { get; set; }
    }

    public class UpcomingSessionsDTO
    {
        public DateTime Date { get; set; }
        public string TopicName { get; set; }
        public double StartTimeHours { get; set; }
    }

    public class TimeDistributionByPlanDTO
    {
        public string PlanName { get; set; }
        public double TotalTime { get; set; }
    }
}
