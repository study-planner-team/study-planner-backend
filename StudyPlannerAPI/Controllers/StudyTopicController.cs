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

        /// <summary>
        /// Pobiera tematy dla planu nauki
        /// </summary>
        /// <response code="200">Zwraca listę tematów dla planu nauki</response>
        [HttpGet]
        [ProducesResponseType(typeof(List<StudyTopicResponseDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTopics(int studyPlanId)
        {
            var topics = await _studyTopicService.GetTopicsForStudyPlan(studyPlanId);
            return Ok(topics);
        }

        /// <summary>
        /// Dodaje nowy temat do planu nauki
        /// </summary>
        /// <response code="200">Zwraca dane dodanego tematu</response>
        /// <response code="400">Jeżeli temat już istnieje w planie nauki lub dane wejściowe są nieprawidłowe</response>
        [HttpPost]
        [ProducesResponseType(typeof(StudyTopicResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddTopic(int studyPlanId, [FromBody] StudyTopicDTO topicDTO)
        {
            var validationResult = await _validator.ValidateAsync(topicDTO);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var addedTopic = await _studyTopicService.AddTopicToStudyPlan(studyPlanId, topicDTO);
            if (addedTopic == null)
            {
                return BadRequest("The topic already exists in the study plan.");
            }

            return Ok(addedTopic);
        }

        /// <summary>
        /// Usuwa temat z planu nauki
        /// </summary>
        /// <response code="200">Jeżeli temat został usunięty</response>
        /// <response code="404">Jeżeli temat nie istnieje</response>
        [HttpDelete("{topicId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteStudyStopic(int topicId)
        {
            var deleted = await _studyTopicService.DeleteStudyTopic(topicId);

            if (!deleted)
                return NotFound();

            return Ok();
        }
    }
}
