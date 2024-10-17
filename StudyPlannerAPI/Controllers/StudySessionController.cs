using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudyPlannerAPI.Models.StudySessions;
using StudyPlannerAPI.Models.Users;
using StudyPlannerAPI.Services.StudySessionsServices;
using System.Security.Claims;

namespace StudyPlannerAPI.Controllers
{
    [Route("api/schedules")]
    [ApiController]
    [Authorize]
    public class StudySessionController : ControllerBase
    {
        private readonly IStudySessionService _studySessionService;

        public StudySessionController(IStudySessionService studySessionService)
        {
            _studySessionService = studySessionService;
        }

        [HttpPost("generate")]
        public async Task<IActionResult> GenerateSchedule([FromBody] StudySessionDTO scheduleData)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            scheduleData.UserId = userId;

            var generatedSchedule = await _studySessionService.GenerateAndStoreSchedule(scheduleData);
            return Ok(generatedSchedule);
        }
    }
}
