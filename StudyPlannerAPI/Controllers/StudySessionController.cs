﻿using FluentValidation;
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

        [HttpGet("current")]
        public async Task<IActionResult> GetCurrentSession()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var session = await _studySessionService.GetCurrentSession(userId);

            if (session == null)
                return NoContent();

            return Ok(session);
        }

        [HttpPost("{sessionId}/start")]
        public async Task<IActionResult> StartSession(int sessionId)
        {
            var updatedSession = await _studySessionService.StartSession(sessionId);

            if (updatedSession==null)
                return BadRequest("Could not start session. Ensure it has not already been started, completed, or missed.");

            return Ok(updatedSession);
        }

        [HttpPost("{sessionId}/end")]
        public async Task<IActionResult> EndSession(int sessionId)
        {
            var updatedSession = await _studySessionService.EndSession(sessionId);

            if (updatedSession == null)
                return BadRequest("Could not end session. Ensure it is in progress.");

            return Ok(updatedSession);
        }

        [HttpGet("completed")]
        public async Task<IActionResult> GetCompletedSessions()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var sessions = await _studySessionService.GetCompletedSessions(userId);

            if (sessions == null)
                return NoContent();

            return Ok(sessions);
        }

        [HttpGet("next")]
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
