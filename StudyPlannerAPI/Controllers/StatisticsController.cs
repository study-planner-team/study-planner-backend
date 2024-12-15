using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudyPlannerAPI.Services.StatisticsService;
using System.Security.Claims;

namespace StudyPlannerAPI.Controllers
{
    [Route("api/statistics")]
    [ApiController]
    [Authorize]
    public class StatisticsController : ControllerBase
    {
        private readonly IStatisticsService _statisticsService;

        public StatisticsController(IStatisticsService statisticsService)
        {
            _statisticsService = statisticsService;
        }

        [HttpGet]
        public async Task<IActionResult> GetStatistics()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var statisticsData = await _statisticsService.GetStatisticsAsync(userId);
            return Ok(statisticsData);
        }
    }
}
