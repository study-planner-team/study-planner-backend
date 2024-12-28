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
        public async Task<IActionResult> GetAssignedQuizzes()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var assignedQuizzes = await _quizService.GetAssignedQuizzes(userId);

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
        public async Task<IActionResult> GetCreatedQuizzes()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var assignedQuizzes = await _quizService.GetCreatedQuizzes(userId);

            return Ok(assignedQuizzes);
        }

        [HttpPut("assigned/{assignmentId}/complete")]
        public async Task<IActionResult> CompleteQuiz(int assignmentId, [FromBody] QuizCompletionDTO completionData)
        {
            var success = await _quizService.UpdateQuizScore(assignmentId, completionData.CorrectAnswers, completionData.TotalQuestions);

            if (!success)
            {
                return NotFound("Quiz assignment not found");
            }

            return NoContent(); 
        }

        [HttpGet("completed")]
        public async Task<IActionResult> GetCompletedQuizzes()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var completedQuizzes = await _quizService.GetCompletedQuizzes(userId);

            return Ok(completedQuizzes);
        }

    }
}
