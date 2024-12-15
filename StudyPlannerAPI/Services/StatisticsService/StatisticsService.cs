using Microsoft.EntityFrameworkCore;
using StudyPlannerAPI.Data;
using StudyPlannerAPI.Models.Statistics;
using StudyPlannerAPI.Models.StudySessions;

namespace StudyPlannerAPI.Services.StatisticsService
{
    public class StatisticsService : IStatisticsService
    {
        private readonly AppDbContext _context;

        public StatisticsService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<CombinedStatisticsDTO> GetStatisticsAsync(int userId)
        {
            var precomputedMetrics = new PrecomputedMetricsDTO
            {
                CompletedSessions = await _context.StudySessions
                        .Where(s => s.UserId == userId && s.Status == StudySessionStatus.Completed)
                        .CountAsync(),

                MissedSessions = await _context.StudySessions
                        .Where(s => s.UserId == userId && s.Status == StudySessionStatus.Missed)
                        .CountAsync(),

                InProgressSessions = await _context.StudySessions
                        .Where(s => s.UserId == userId && s.Status == StudySessionStatus.InProgress)
                        .CountAsync(),

                ActivePlans = await _context.StudyPlans
                        .Where(p => p.UserId == userId && !p.IsArchived)
                        .CountAsync(),

                ArchivedPlans = await _context.StudyPlans
                        .Where(p => p.UserId == userId && p.IsArchived)
                        .CountAsync(),

                JoinedPlans = await _context.StudyPlanMembers
                        .Where(jp => jp.UserId == userId && !_context.StudyPlans
                            .Any(sp => sp.StudyPlanId == jp.StudyPlanId && sp.UserId == userId))
                        .CountAsync()
            };

            var aggregatedStatistics = new AggregatedStatisticsDTO
            {
                SessionsByDay = await _context.StudySessions
                    .Where(s => s.UserId == userId && s.Status == StudySessionStatus.Completed)
                    .GroupBy(s => s.Date)
                    .Select(g => new SessionsByDayDTO
                    {
                        Date = g.Key,
                        Count = g.Count()
                    })
                    .ToListAsync(),

                SessionsMissedByDay = await _context.StudySessions
                    .Where(s => s.UserId == userId && s.Status == StudySessionStatus.Missed)
                    .GroupBy(s => s.Date)
                    .Select(g => new SessionsByDayDTO
                    {
                        Date = g.Key,
                        Count = g.Count()
                    })
                    .ToListAsync(),

                TimeDistribution = await _context.StudyPlans
                    .Where(p => p.UserId == userId && !p.IsArchived)
                    .SelectMany(p => p.StudyTopics)
                    .GroupBy(t => t.Title)
                    .Select(g => new TimeDistributionDTO
                    {
                        TopicName = g.Key,
                        TotalTime = Math.Round(g.Sum(t => t.Hours), 2)
                    })
                    .ToListAsync(),

                DurationTrends = _context.StudySessions
                    .Where(s => s.UserId == userId
                                && s.Status == StudySessionStatus.Completed
                                && s.ActualDuration != null)
                    .AsEnumerable()
                    .GroupBy(s => s.Date)
                    .Select(g => new DurationTrendsDTO
                    {
                        Date = g.Key,
                        TotalActualDuration = Math.Round(g.Sum(s => s.ActualDuration.Value.TotalMinutes))
                    })
                    .ToList(),

                ProgressTowardGoals = await _context.StudyPlans
                    .Where(p => p.UserId == userId && !p.IsArchived)
                    .Select(plan => new ProgressTowardGoalsDTO
                    {
                        PlanName = plan.Title,
                        CompletionPercentage = Math.Round((double)_context.StudySessions.Count(s => s.StudyPlanId == plan.StudyPlanId && s.Status == StudySessionStatus.Completed) /
                            Math.Max(1, _context.StudySessions.Count(s => s.StudyPlanId == plan.StudyPlanId)) * 100, 2)
                    })
                    .ToListAsync(),

                PreferredStudyTimes = await _context.StudySessions
                    .Where(s => s.UserId == userId)
                    .GroupBy(s => s.StartTime)
                    .Select(g => new PreferredStudyTimesDTO
                    {
                        Hour = g.Key.ToString(@"hh\:mm"),
                        Count = g.Count()
                    })
                    .ToListAsync(),

                UpcomingSessions = await _context.StudySessions
                    .Where(s => s.UserId == userId && s.Date >= DateTime.Now.Date)
                    .Select(s => new UpcomingSessionsDTO
                    {
                        Date = s.Date,
                        TopicName = s.StudyTopic.Title,
                        StartTimeHours = s.StartTime.TotalHours
                    })
                    .ToListAsync(),

                TimeDistributionByPlan = await _context.StudyPlans
                    .Where(p => p.UserId == userId && !p.IsArchived)
                    .Select(plan => new TimeDistributionByPlanDTO
                    {
                        PlanName = plan.Title,
                        TotalTime = Math.Round(
                            plan.StudySessions.Any()
                                ? plan.StudySessions.Sum(s => s.Duration) / 60.0 // Sum durations from sessions if they exist
                                : plan.StudyTopics.Sum(t => t.Hours), 2) // Sum hours from study topics if no sessions exist
                    })
                    .ToListAsync()
            };

            return new CombinedStatisticsDTO
            {
                PrecomputedMetrics = precomputedMetrics,
                AggregatedStatistics = aggregatedStatistics
            };
        }
    }
}
