using StudyPlannerAPI.Models.Statistics;

namespace StudyPlannerAPI.Services.StatisticsService
{
    public interface IStatisticsService
    {
        Task<CombinedStatisticsDTO> GetStatisticsAsync(int userId);
    }
}
