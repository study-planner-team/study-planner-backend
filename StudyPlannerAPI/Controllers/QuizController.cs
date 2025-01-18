using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using StudyPlannerAPI.Models.Quizes.RequestDTOs;
using StudyPlannerAPI.Models.StudyPlans;
using StudyPlannerAPI.Services.QuizService;
using StudyPlannerAPI.Validators.StudyPlanValidators;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace StudyPlannerAPI.Controllers
{
    [ApiController]
    [Route("api/studyplans/{studyPlanId}/quizzes")]
    public class QuizController : ControllerBase
    {
        private readonly IQuizService _quizService;
        private readonly IValidator<QuizRequestDTO> _quizValidator;
        public QuizController(IQuizService quizService, IValidator<QuizRequestDTO> quizValidator)
        {
            _quizService = quizService;
            _quizValidator = quizValidator;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateQuizWithQuestions(int studyPlanId, [FromBody] QuizRequestDTO quizDto)
        {
            var validationResult = await _quizValidator.ValidateAsync(quizDto);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var createdQuiz = await _quizService.CreateQuizWithQuestions(studyPlanId, quizDto, userId);
            return Ok(createdQuiz);
        }

        [HttpGet("{quizId}")]
        public async Task<IActionResult> GetQuizById(int quizId)
        {
            var quiz = await _quizService.GetQuizById(quizId);
            if (quiz == null)
            {
                return NotFound();
            }

            return Ok(quiz);
        }

        [HttpDelete("{quizId}")]
        public async Task<IActionResult> DeleteQuiz(int quizId)
        {
            var success = await _quizService.DeleteQuiz(quizId);
            if (!success)
            {
                return NotFound("Quiz not found");
            }

            return NoContent();
        }

        [HttpPost("{quizId}/assign")]
        public async Task<IActionResult> AssignQuiz(int quizId, [FromBody] int userId)
        {
            var success = await _quizService.AssignQuizToUser(quizId, userId);
            if (!success)
            {
                return BadRequest("Quiz is already assigned to this user.");
            }

            return Ok("Quiz assigned successfully.");
        }

        [HttpGet("assigned")]
        public async Task<IActionResult> GetAssignedQuizzes(int studyPlanId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var assignedQuizzes = await _quizService.GetAssignedQuizzes(userId, studyPlanId);

            return Ok(assignedQuizzes);
        }

        [HttpGet("assigned/{quizId}")]
        public async Task<IActionResult> GetAssignedQuizById(int quizId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var assignedQuiz = await _quizService.GetAssignedQuizById(userId, quizId);

            if (assignedQuiz == null)
            {
                return NotFound();
            }

            return Ok(assignedQuiz);
        }

        [HttpGet("created")]
        public async Task<IActionResult> GetCreatedQuizzes(int studyPlanId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var createdQuizzes = await _quizService.GetCreatedQuizzes(userId, studyPlanId);

            return Ok(createdQuizzes);
        }

        [HttpPut("assigned/{assignmentId}/complete")]
        public async Task<IActionResult> CompleteQuiz(int assignmentId, [FromBody] QuizCompletionDTO completionData)
        {
            var updatedAssignment = await _quizService.UpdateQuizScore(assignmentId, completionData.Answers);

            if (updatedAssignment == null)
            {
                return NotFound("Quiz assignment not found");
            }

            // Return 200 OK with the updated assignment
            return Ok(updatedAssignment);
        }

        [HttpGet("completed")]
        public async Task<IActionResult> GetCompletedQuizzes(int studyPlanId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var completedQuizzes = await _quizService.GetCompletedQuizzes(userId, studyPlanId);

            return Ok(completedQuizzes);
        }

    }
}
