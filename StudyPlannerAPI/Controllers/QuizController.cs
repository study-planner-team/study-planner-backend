using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using StudyPlannerAPI.Models.Quizes.RequestDTOs;
using StudyPlannerAPI.Models.Quizes.ResponseDTOs;
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

        /// <summary>
        /// Tworzy nowy quiz
        /// </summary>
        /// <response code="200">Zwraca nowo utworzony quiz</response>
        /// <response code="400">Jeżeli wystąpią błędy w walidacji</response>
        [HttpPost("create")]
        [ProducesResponseType(typeof(QuizResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
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

        /// <summary>
        /// Pobiera quiz według identyfikatora
        /// </summary>
        /// <response code="200">Zwraca dane quizu</response>
        /// <response code="404">Jeżeli quiz o podanym identyfikatorze nie istnieje</response>
        [HttpGet("{quizId}")]
        [ProducesResponseType(typeof(QuizResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetQuizById(int quizId)
        {
            var quiz = await _quizService.GetQuizById(quizId);
            if (quiz == null)
            {
                return NotFound();
            }

            return Ok(quiz);
        }

        /// <summary>
        /// Usuwa quiz według identyfikatora
        /// </summary>
        /// <response code="204">Jeżeli quiz został usunięty</response>
        /// <response code="404">Jeżeli quiz o podanym identyfikatorze nie istnieje</response>
        [HttpDelete("{quizId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteQuiz(int quizId)
        {
            var success = await _quizService.DeleteQuiz(quizId);
            if (!success)
            {
                return NotFound("Quiz not found");
            }

            return NoContent();
        }

        /// <summary>
        /// Przypisuje quiz użytkownikowi
        /// </summary>
        /// <response code="200">Jeżeli przypisanie zakończyło się sukcesem</response>
        /// <response code="400">Jeżeli quiz jest już przypisany do użytkownika</response>
        [HttpPost("{quizId}/assign")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AssignQuiz(int quizId, [FromBody] int userId)
        {
            var success = await _quizService.AssignQuizToUser(quizId, userId);
            if (!success)
            {
                return BadRequest("Quiz is already assigned to this user.");
            }

            return Ok("Quiz assigned successfully.");
        }

        /// <summary>
        /// Pobiera przypisane quizy
        /// </summary>
        /// <response code="200">Zwraca listę przypisanych quizów</response>
        [HttpGet("assigned")]
        [ProducesResponseType(typeof(IEnumerable<QuizAssignmentResponseDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAssignedQuizzes(int studyPlanId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var assignedQuizzes = await _quizService.GetAssignedQuizzes(userId, studyPlanId);

            return Ok(assignedQuizzes);
        }

        /// <summary>
        /// Pobiera przypisany quiz według identyfikatora
        /// </summary>
        /// <response code="200">Zwraca dane przypisanego quizu</response>
        /// <response code="404">Jeżeli przypisany quiz nie istnieje</response>
        [HttpGet("assigned/{quizId}")]
        [ProducesResponseType(typeof(QuizAssignmentResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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

        /// <summary>
        /// Pobiera utworzone quizy
        /// </summary>
        /// <response code="200">Zwraca listę utworzonych quizów</response>
        [HttpGet("created")]
        [ProducesResponseType(typeof(IEnumerable<QuizResponseDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCreatedQuizzes(int studyPlanId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var createdQuizzes = await _quizService.GetCreatedQuizzes(userId, studyPlanId);

            return Ok(createdQuizzes);
        }

        /// <summary>
        /// Oznacza quiz jako ukończony
        /// </summary>
        /// <response code="200">Zwraca dane zaktualizowanego przypisania quizu</response>
        /// <response code="404">Jeżeli przypisanie quizu nie istnieje</response>
        [HttpPut("assigned/{assignmentId}/complete")]
        [ProducesResponseType(typeof(QuizAssignmentResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CompleteQuiz(int assignmentId, [FromBody] QuizCompletionDTO completionData)
        {
            var updatedAssignment = await _quizService.UpdateQuizScore(assignmentId, completionData.Answers);

            if (updatedAssignment == null)
            {
                return NotFound("Quiz assignment not found");
            }

            return Ok(updatedAssignment);
        }

        /// <summary>
        /// Pobiera ukończone quizy
        /// </summary>
        /// <response code="200">Zwraca listę ukończonych quizów</response>
        [HttpGet("completed")]
        [ProducesResponseType(typeof(IEnumerable<QuizAssignmentResponseDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCompletedQuizzes(int studyPlanId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var completedQuizzes = await _quizService.GetCompletedQuizzes(userId, studyPlanId);

            return Ok(completedQuizzes);
        }
    }
}
