using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudyPlannerAPI.Models.StudyPlans;
using StudyPlannerAPI.Models.StudySessions;
using StudyPlannerAPI.Models.Users;
using StudyPlannerAPI.Services.StudySessionsServices;
using StudyPlannerAPI.Validators.StudyPlanValidators;
using System.Security.Claims;

namespace StudyPlannerAPI.Controllers
{
    [Route("api/schedules")]
    [ApiController]
    [Authorize]
    public class StudySessionController : ControllerBase
    {
        private readonly IStudySessionService _studySessionService;
        private readonly IValidator<StudySessionDTO> _studySessionValidator;
        public StudySessionController(IStudySessionService studySessionService, IValidator<StudySessionDTO> studySessionValidator)
        {
            _studySessionService = studySessionService;
            _studySessionValidator = studySessionValidator;
        }

        [HttpPost("generate")]
        public async Task<IActionResult> GenerateSchedule([FromBody] StudySessionDTO scheduleData)
        {
            var validationResult = await _studySessionValidator.ValidateAsync(scheduleData);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            scheduleData.UserId = userId;

            var generatedSchedule = await _studySessionService.GenerateAndStoreSchedule(scheduleData);
            if (generatedSchedule == null)
            {
                return BadRequest("The schedule could not be generated within the specified date range. Please extend the end date, increase session length, or allow more sessions per day.");
            }

            return Ok(generatedSchedule);
        }

        [HttpGet]
        public async Task<IActionResult> GetUserStudySessions()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var sessions = await _studySessionService.GetUserStudySessions(userId);
            return Ok(sessions);
        }

        [HttpDelete("{sessionId}")]
        public async Task<IActionResult> DeleteStudySession(int sessionId)
        {

            var deleted = await _studySessionService.DeleteStudySession(sessionId);

            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}
