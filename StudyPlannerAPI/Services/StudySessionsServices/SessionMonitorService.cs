
namespace StudyPlannerAPI.Services.StudySessionsServices
{
    public class SessionMonitorService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public SessionMonitorService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var sessionService = scope.ServiceProvider.GetRequiredService<IStudySessionService>();
                    await sessionService.MarkExpiredSessionsAsync();
                }

                // Run every 1 minute
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }
}
