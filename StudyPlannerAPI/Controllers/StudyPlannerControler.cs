using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudyPlannerAPI.Models.StudyPlans;
using StudyPlannerAPI.Services.StudyPlanServices;
using System.Security.Claims;

namespace StudyPlannerAPI.Controllers
{
    [Route("api/studyplans")]
    [ApiController]
    [Authorize]
    public class StudyPlannerController : ControllerBase
    {
        private readonly IStudyPlanService _studyPlanService;
        private readonly IValidator<StudyPlanDTO> _studyPlanValidator;
        public StudyPlannerController(IStudyPlanService studyPlanService, IValidator<StudyPlanDTO> studyPlanValidator)
        {
            _studyPlanService = studyPlanService;
            _studyPlanValidator = studyPlanValidator;
        }

        [HttpPost]
        public async Task<IActionResult> CreateStudyPlan([FromBody] StudyPlanDTO studyPlanDTO)
        {
            var validationResult = await _studyPlanValidator.ValidateAsync(studyPlanDTO);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var studyPlan = await _studyPlanService.CreateStudyPlan(userId, studyPlanDTO);

            return Ok(studyPlan);
        }

        [HttpGet]
        public async Task<IActionResult> GetStudyPlansForUser()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var studyPlans = await _studyPlanService.GetStudyPlansForUser(userId);
            return Ok(studyPlans);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetStudyPlanById(int id)
        {
            var studyPlan = await _studyPlanService.GetStudyPlanById(id);

            if (studyPlan == null)
                return NotFound("Study plan not found.");

            return Ok(studyPlan);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStudyPlan(int id, [FromBody] StudyPlanDTO studyPlanDTO)
        {
            var validationResult = await _studyPlanValidator.ValidateAsync(studyPlanDTO);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var updatedStudyPlan = await _studyPlanService.UpdateStudyPlan(id, studyPlanDTO);

            if (updatedStudyPlan == null)
                return NotFound("Study plan not found.");

            return Ok(updatedStudyPlan);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStudyPlan(int id)
        {
            var deleted = await _studyPlanService.DeleteStudyPlan(id);

            if (!deleted)
                return NotFound("Study plan not found.");

            return NoContent();
        }

        [HttpPut("{id}/archive")]
        public async Task<IActionResult> ArchiveStudyPlan(int id)
        {
            var archivedPlan = await _studyPlanService.ArchiveStudyPlan(id);

            if (archivedPlan == null)
                return NotFound("Study plan not found.");

            return Ok(archivedPlan);
        }

        [HttpPut("{id}/unarchive")]
        public async Task<IActionResult> UnarchiveStudyPlan(int id)
        {
            var unarchivedPlan = await _studyPlanService.UnarchiveStudyPlan(id);

            if (unarchivedPlan == null)
                return NotFound("Study plan not found.");

            return Ok(unarchivedPlan);
        }

        [HttpGet("archived")]
        public async Task<IActionResult> GetArchivedStudyPlansForUser()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var archivedPlans = await _studyPlanService.GetArchivedStudyPlansForUser(userId);

            return Ok(archivedPlans);
        }
    }
}
