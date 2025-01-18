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

        /// <summary>
        /// Generuje harmonogram sesji nauki
        /// </summary>
        /// <response code="200">Zwraca wygenerowany harmonogram</response>
        /// <response code="400">Jeżeli harmonogram nie mógł zostać wygenerowany</response>
        [HttpPost("generate")]
        [ProducesResponseType(typeof(List<StudySession>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
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

        /// <summary>
        /// Pobiera wszystkie sesje nauki użytkownika
        /// </summary>
        /// <response code="200">Zwraca listę sesji nauki</response>
        [HttpGet]
        [ProducesResponseType(typeof(List<StudySessionResponseDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUserStudySessions()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var sessions = await _studySessionService.GetUserStudySessions(userId);
            return Ok(sessions);
        }

        /// <summary>
        /// Usuwa sesję nauki
        /// </summary>
        /// <response code="204">Jeżeli sesja została usunięta</response>
        /// <response code="404">Jeżeli sesja nie istnieje</response>
        [HttpDelete("{sessionId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteStudySession(int sessionId)
        {
            var deleted = await _studySessionService.DeleteStudySession(sessionId);

            if (!deleted)
                return NotFound();

            return NoContent();
        }

        /// <summary>
        /// Pobiera aktualną sesję nauki
        /// </summary>
        /// <response code="200">Zwraca aktualną sesję nauki</response>
        /// <response code="204">Jeżeli brak aktualnej sesji</response>
        [HttpGet("current")]
        [ProducesResponseType(typeof(StudySessionResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> GetCurrentSession()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var session = await _studySessionService.GetCurrentSession(userId);

            if (session == null)
                return NoContent();

            return Ok(session);
        }

        /// <summary>
        /// Rozpoczyna sesję nauki
        /// </summary>
        /// <response code="200">Zwraca dane zaktualizowanej sesji</response>
        /// <response code="400">Jeżeli sesja nie może zostać rozpoczęta</response>
        [HttpPost("{sessionId}/start")]
        [ProducesResponseType(typeof(StudySessionResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> StartSession(int sessionId)
        {
            var updatedSession = await _studySessionService.StartSession(sessionId);

            if (updatedSession == null)
                return BadRequest("Could not start session. Ensure it has not already been started, completed, or missed.");

            return Ok(updatedSession);
        }

        /// <summary>
        /// Kończy sesję nauki
        /// </summary>
        /// <response code="200">Zwraca dane zaktualizowanej sesji</response>
        /// <response code="400">Jeżeli sesja nie może zostać zakończona</response>
        [HttpPost("{sessionId}/end")]
        [ProducesResponseType(typeof(StudySessionResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> EndSession(int sessionId)
        {
            var updatedSession = await _studySessionService.EndSession(sessionId);

            if (updatedSession == null)
                return BadRequest("Could not end session. Ensure it is in progress.");

            return Ok(updatedSession);
        }

        /// <summary>
        /// Pobiera ukończone sesje nauki
        /// </summary>
        /// <response code="200">Zwraca listę ukończonych sesji nauki</response>
        /// <response code="204">Jeżeli brak ukończonych sesji</response>
        [HttpGet("completed")]
        [ProducesResponseType(typeof(List<StudySessionResponseDTO?>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> GetCompletedSessions()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var sessions = await _studySessionService.GetCompletedSessions(userId);

            if (sessions == null || !sessions.Any())
                return NoContent();

            return Ok(sessions);
        }

        /// <summary>
        /// Pobiera następne sesje nauki
        /// </summary>
        /// <response code="200">Zwraca dane następnej sesji nauki</response>
        /// <response code="204">Jeżeli brak następnych sesji</response>
        [HttpGet("next")]
        [ProducesResponseType(typeof(StudySessionResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> GetNextSessions()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var sessions = await _studySessionService.GetNextSession(userId);

            if (sessions == null)
                return NoContent();

            return Ok(sessions);
        }
    }
}
