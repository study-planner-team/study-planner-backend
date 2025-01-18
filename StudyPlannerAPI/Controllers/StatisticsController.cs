using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudyPlannerAPI.Models.Statistics;
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

        /// <summary>
        /// Pobiera statystyki nauki dla użytkownika
        /// </summary>
        /// <response code="200">Zwraca statystyki nauki dla użytkownika</response>
        [HttpGet]
        [ProducesResponseType(typeof(CombinedStatisticsDTO), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetStatistics()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var statisticsData = await _statisticsService.GetStatisticsAsync(userId);
            return Ok(statisticsData);
        }
    }
}
