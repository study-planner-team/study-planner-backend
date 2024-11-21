using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudyPlannerAPI.Models.StudyTopics;
using StudyPlannerAPI.Services.StudyTopicServices;

namespace StudyPlannerAPI.Controllers
{
    [Route("api/studyplans/{studyPlanId}/topics")]
    [ApiController]
    [Authorize]
    public class StudyTopicController : ControllerBase
    {
        private readonly IStudyTopicService _studyTopicService;
        private readonly IValidator<StudyTopicDTO> _validator;

        public StudyTopicController(IStudyTopicService studyTopicService, IValidator<StudyTopicDTO> validator)
        {
            _studyTopicService = studyTopicService;
            _validator = validator;
        }

        [HttpGet]
        public async Task<IActionResult> GetTopics(int studyPlanId)
        {
            var topics = await _studyTopicService.GetTopicsForStudyPlan(studyPlanId);
            return Ok(topics);
        }

        [HttpPost]
        public async Task<IActionResult> AddTopic(int studyPlanId, [FromBody] StudyTopicDTO topicDTO)
        {
            var validationResult = await _validator.ValidateAsync(topicDTO);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var addedTopic = await _studyTopicService.AddTopicToStudyPlan(studyPlanId, topicDTO);
            return Ok(addedTopic);
        }

        [HttpDelete("{topicId}")]
        public async Task<IActionResult> DeleteStudyStopic(int topicId)
        {
            var deleted = await _studyTopicService.DeleteStudyTopic(topicId);

            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}