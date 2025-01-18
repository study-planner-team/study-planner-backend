namespace StudyPlannerAPI.Models.Statistics
{
    public class CombinedStatisticsDTO
    {
        public PrecomputedMetricsDTO PrecomputedMetrics { get; set; }
        public AggregatedStatisticsDTO AggregatedStatistics { get; set; }
    }
}
