using Microsoft.EntityFrameworkCore;
using StudyPlannerAPI.Data;
using StudyPlannerAPI.Models.Quizes;
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
            double averageQuizScore = await CalculateAverageQuizScore(userId);
            var quizScoreDistribution = await BuildQuizScoreDistribution(userId);

            var precomputedMetrics = await BuildPrecomputedMetricsDTO(userId, averageQuizScore);
            var aggregatedStatistics = await BuildAggregatedStatisticsDTO(userId, quizScoreDistribution);

            return new CombinedStatisticsDTO
            {
                PrecomputedMetrics = precomputedMetrics,
                AggregatedStatistics = aggregatedStatistics
            };
        }

        private async Task<double> CalculateAverageQuizScore(int userId)
        {
            var completedAssignments = await _context.QuizAssignments
                .Where(a => a.AssignedToUserId == userId
                            && a.State == QuizState.Completed
                            && a.TotalQuestions.HasValue
                            && a.TotalQuestions.Value > 0)
                .ToListAsync();

            if (completedAssignments.Count == 0)
                return 0;

            double average = completedAssignments.Average(a =>
                (double)a.CorrectAnswers.Value / a.TotalQuestions.Value * 100
            );

            return Math.Round(average, 2);
        }

        private async Task<List<ScoreDistributionDTO>> BuildQuizScoreDistribution(int userId)
        {
            var completedQuizzes = await _context.QuizAssignments
                .Where(a => a.AssignedToUserId == userId
                            && a.State == QuizState.Completed
                            && a.TotalQuestions.HasValue
                            && a.TotalQuestions.Value > 0)
                .Select(a => new
                {
                    Score = (double)a.CorrectAnswers.Value / a.TotalQuestions.Value * 100
                })
                .ToListAsync();

            var quizDistributionDict = new Dictionary<string, int>
            {
                { "0–49", 0 },
                { "50–69", 0 },
                { "70–84", 0 },
                { "85–100", 0 }
            };

            foreach (var quiz in completedQuizzes)
            {
                double score = quiz.Score;
                if (score < 50) quizDistributionDict["0–49"]++;
                else if (score < 70) quizDistributionDict["50–69"]++;
                else if (score < 85) quizDistributionDict["70–84"]++;
                else quizDistributionDict["85–100"]++;
            }

            return quizDistributionDict.Select(q => new ScoreDistributionDTO { Bucket = q.Key, Count = q.Value }).ToList();
        }

        private async Task<PrecomputedMetricsDTO> BuildPrecomputedMetricsDTO(int userId, double averageQuizScore)
        {
            int completedSessions = await _context.StudySessions
                .Where(s => s.UserId == userId && s.Status == StudySessionStatus.Completed)
                .CountAsync();

            int missedSessions = await _context.StudySessions
                .Where(s => s.UserId == userId && s.Status == StudySessionStatus.Missed)
                .CountAsync();

            int inProgressSessions = await _context.StudySessions
                .Where(s => s.UserId == userId && s.Status == StudySessionStatus.InProgress)
                .CountAsync();

            int activePlans = await _context.StudyPlans
                .Where(p => p.UserId == userId && !p.IsArchived)
                .CountAsync();

            int archivedPlans = await _context.StudyPlans
                .Where(p => p.UserId == userId && p.IsArchived)
                .CountAsync();

            int joinedPlans = await _context.StudyPlanMembers
                .Where(jp => jp.UserId == userId && !_context.StudyPlans
                    .Any(sp => sp.StudyPlanId == jp.StudyPlanId && sp.UserId == userId))
                .CountAsync();

            int assignedQuizCount = await _context.QuizAssignments
                .Where(a => a.AssignedToUserId == userId && a.State == QuizState.Assigned)
                .CountAsync();

            int completedQuizCount = await _context.QuizAssignments
                .Where(a => a.AssignedToUserId == userId && a.State == QuizState.Completed)
                .CountAsync();

            return new PrecomputedMetricsDTO
            {
                CompletedSessions = completedSessions,
                MissedSessions = missedSessions,
                InProgressSessions = inProgressSessions,
                ActivePlans = activePlans,
                ArchivedPlans = archivedPlans,
                JoinedPlans = joinedPlans,
                AssignedQuizCount = assignedQuizCount,
                CompletedQuizCount = completedQuizCount,
                AverageQuizScore = averageQuizScore
            };
        }

        private async Task<AggregatedStatisticsDTO> BuildAggregatedStatisticsDTO(
            int userId,
            List<ScoreDistributionDTO> quizScoreDistribution)
        {
            var sessionsByDay = await _context.StudySessions
                .Where(s => s.UserId == userId && s.Status == StudySessionStatus.Completed)
                .GroupBy(s => s.Date)
                .Select(g => new SessionsByDayDTO
                {
                    Date = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();

            var sessionsMissedByDay = await _context.StudySessions
                .Where(s => s.UserId == userId && s.Status == StudySessionStatus.Missed)
                .GroupBy(s => s.Date)
                .Select(g => new SessionsByDayDTO
                {
                    Date = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();

            var timeDistribution = await _context.StudyPlans
                .Where(p => p.UserId == userId && !p.IsArchived)
                .SelectMany(p => p.StudyTopics)
                .GroupBy(t => t.Title)
                .Select(g => new TimeDistributionDTO
                {
                    TopicName = g.Key,
                    TotalTime = Math.Round(g.Sum(t => t.Hours), 2)
                })
                .ToListAsync();

            var durationTrends = _context.StudySessions
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
                .ToList();

            var progressTowardGoals = await _context.StudyPlans
                .Where(p => p.UserId == userId && !p.IsArchived)
                .Select(plan => new ProgressTowardGoalsDTO
                {
                    PlanName = plan.Title,
                    CompletionPercentage = Math.Round(
                        (double)_context.StudySessions.Count(s => s.StudyPlanId == plan.StudyPlanId && s.Status == StudySessionStatus.Completed) /
                        Math.Max(1, _context.StudySessions.Count(s => s.StudyPlanId == plan.StudyPlanId)) * 100, 2)
                })
                .ToListAsync();

            var preferredStudyTimes = await _context.StudySessions
                .Where(s => s.UserId == userId)
                .GroupBy(s => s.StartTime)
                .Select(g => new PreferredStudyTimesDTO
                {
                    Hour = g.Key.ToString(@"hh\:mm"),
                    Count = g.Count()
                })
                .ToListAsync();

            var upcomingSessions = await _context.StudySessions
                .Where(s => s.UserId == userId && s.Date >= DateTime.Now.Date)
                .Select(s => new UpcomingSessionsDTO
                {
                    Date = s.Date,
                    TopicName = s.StudyTopic.Title,
                    StartTimeHours = s.StartTime.TotalHours
                })
                .ToListAsync();

            var timeDistributionByPlan = await _context.StudyPlans
                .Where(p => p.UserId == userId && !p.IsArchived)
                .Select(plan => new TimeDistributionByPlanDTO
                {
                    PlanName = plan.Title,
                    TotalTime = Math.Round(
                        plan.StudySessions.Any()
                            ? plan.StudySessions.Sum(s => s.Duration) / 60.0
                            : plan.StudyTopics.Sum(t => t.Hours),
                        2)
                })
                .ToListAsync();

            var quizCompletionsOverTime = await _context.QuizAssignments
                .Where(a => a.AssignedToUserId == userId
                            && a.State == QuizState.Completed
                            && a.CompletedOn.HasValue)
                .GroupBy(a => a.CompletedOn.Value.Date)
                .Select(g => new QuizCompletionOverTimeDTO
                {
                    Date = g.Key,
                    Count = g.Count()
                })
                .OrderBy(x => x.Date)
                .ToListAsync();

            return new AggregatedStatisticsDTO
            {
                SessionsByDay = sessionsByDay,
                SessionsMissedByDay = sessionsMissedByDay,
                TimeDistribution = timeDistribution,
                DurationTrends = durationTrends,
                ProgressTowardGoals = progressTowardGoals,
                PreferredStudyTimes = preferredStudyTimes,
                UpcomingSessions = upcomingSessions,
                TimeDistributionByPlan = timeDistributionByPlan,
                QuizCompletionsOverTime = quizCompletionsOverTime,
                QuizScoreDistribution = quizScoreDistribution
            };
        }
    }
}
