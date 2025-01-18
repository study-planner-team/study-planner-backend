using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudyPlannerAPI.Models.StudyPlans;
using StudyPlannerAPI.Models.Users;
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

        /// <summary>
        /// Tworzy nowy plan nauki
        /// </summary>
        /// <response code="200">Zwraca utworzony plan nauki</response>
        /// <response code="400">Jeżeli nazwa planu nie jest unikalna</response>
        [HttpPost]
        [ProducesResponseType(typeof(StudyPlanResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateStudyPlan([FromBody] StudyPlanDTO studyPlanDTO)
        {
            var validationResult = await _studyPlanValidator.ValidateAsync(studyPlanDTO);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var studyPlan = await _studyPlanService.CreateStudyPlan(userId, studyPlanDTO);

            if (studyPlan == null)
            {
                return BadRequest("The plan name is not unique based on the visibility settings.");
            }

            return Ok(studyPlan);
        }

        /// <summary>
        /// Pobiera plany nauki użytkownika
        /// </summary>
        /// <response code="200">Zwraca listę planów nauki użytkownika</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<StudyPlanResponseDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetStudyPlansForUser()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var studyPlans = await _studyPlanService.GetStudyPlansForUser(userId);
            return Ok(studyPlans);
        }

        /// <summary>
        /// Pobiera plan nauki według identyfikatora
        /// </summary>
        /// <response code="200">Zwraca plan nauki</response>
        /// <response code="404">Jeżeli plan nauki nie istnieje</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(StudyPlanResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetStudyPlanById(int id)
        {
            var studyPlan = await _studyPlanService.GetStudyPlanById(id);

            if (studyPlan == null)
                return NotFound("Study plan not found");

            return Ok(studyPlan);
        }

        /// <summary>
        /// Aktualizuje plan nauki
        /// </summary>
        /// <response code="200">Zwraca zaktualizowany plan nauki</response>
        /// <response code="404">Jeżeli plan nauki nie istnieje</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(StudyPlanResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateStudyPlan(int id, [FromBody] StudyPlanDTO studyPlanDTO)
        {
            var validationResult = await _studyPlanValidator.ValidateAsync(studyPlanDTO);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            var updatedStudyPlan = await _studyPlanService.UpdateStudyPlan(id, studyPlanDTO);

            if (updatedStudyPlan == null)
                return NotFound("Study plan not found");

            return Ok(updatedStudyPlan);
        }

        /// <summary>
        /// Usuwa plan nauki
        /// </summary>
        /// <response code="204">Jeżeli plan nauki został usunięty</response>
        /// <response code="404">Jeżeli plan nauki nie istnieje</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteStudyPlan(int id)
        {
            var deleted = await _studyPlanService.DeleteStudyPlan(id);

            if (!deleted)
                return NotFound("Study plan not found");

            return NoContent();
        }

        /// <summary>
        /// Archiwizuje plan nauki
        /// </summary>
        /// <response code="200">Zwraca zarchiwizowany plan nauki</response>
        /// <response code="404">Jeżeli plan nauki nie istnieje</response>
        [HttpPut("{id}/archive")]
        [ProducesResponseType(typeof(StudyPlanResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ArchiveStudyPlan(int id)
        {
            var archivedPlan = await _studyPlanService.ArchiveStudyPlan(id);

            if (archivedPlan == null)
                return NotFound("Study plan not found");

            return Ok(archivedPlan);
        }

        /// <summary>
        /// Usuwa plan nauki z archiwum
        /// </summary>
        /// <response code="200">Zwraca odarchiwizowany plan nauki</response>
        /// <response code="404">Jeżeli plan nauki nie istnieje</response>
        [HttpPut("{id}/unarchive")]
        [ProducesResponseType(typeof(StudyPlanResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UnarchiveStudyPlan(int id)
        {
            var unarchivedPlan = await _studyPlanService.UnarchiveStudyPlan(id);

            if (unarchivedPlan == null)
                return NotFound("Study plan not found");

            return Ok(unarchivedPlan);
        }

        /// <summary>
        /// Pobiera zarchiwizowane plany nauki użytkownika
        /// </summary>
        /// <response code="200">Zwraca listę zarchiwizowanych planów nauki</response>
        [HttpGet("archived")]
        [ProducesResponseType(typeof(IEnumerable<StudyPlanResponseDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetArchivedStudyPlansForUser()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var archivedPlans = await _studyPlanService.GetArchivedStudyPlansForUser(userId);

            return Ok(archivedPlans);
        }

        /// <summary>
        /// Pobiera publiczne plany nauki
        /// </summary>
        /// <response code="200">Zwraca listę publicznych planów nauki</response>
        [HttpGet("public")]
        [ProducesResponseType(typeof(IEnumerable<StudyPlanResponseDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPublicStudyPlans()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var publicPlans = await _studyPlanService.GetPublicStudyPlans(userId);

            return Ok(publicPlans);
        }

        /// <summary>
        /// Pobiera plany nauki, do których użytkownik dołączył
        /// </summary>
        /// <response code="200">Zwraca listę dołączonych planów nauki</response>
        [HttpGet("joined")]
        [ProducesResponseType(typeof(IEnumerable<StudyPlanResponseDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetJoinedStudyPlansForUser()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var joinedPlans = await _studyPlanService.GetJoinedStudyPlansForUser(userId);

            return Ok(joinedPlans);
        }

        /// <summary>
        /// Dołącza użytkownika do publicznego planu nauki
        /// </summary>
        /// <response code="200">Jeżeli użytkownik dołączył do planu nauki</response>
        /// <response code="400">Jeżeli użytkownik nie może dołączyć do planu nauki</response>
        [HttpPost("{studyPlanId}/join")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> JoinPublicStudyPlan(int studyPlanId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var success = await _studyPlanService.JoinPublicStudyPlan(userId, studyPlanId);

            if (!success)
            {
                return BadRequest("Could not join the study plan. User might already be a member or the plan is not public.");
            }

            return Ok("Successfully joined the study plan.");
        }

        /// <summary>
        /// Pobiera listę członków planu nauki
        /// </summary>
        /// <response code="200">Zwraca listę członków planu nauki</response>
        [HttpGet("{studyPlanId}/members")]
        [ProducesResponseType(typeof(IEnumerable<UserResponseDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetStudyPlanMembers(int studyPlanId)
        {
            var members = await _studyPlanService.GetStudyPlanMembers(studyPlanId);
            return Ok(members);
        }

        /// <summary>
        /// Użytkownik opuszcza publiczny plan nauki
        /// </summary>
        /// <response code="200">Jeżeli użytkownik opuścił plan nauki</response>
        /// <response code="404">Jeżeli użytkownik nie jest członkiem planu nauki</response>
        [HttpPost("{studyPlanId}/leave")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> LeavePublicStudyPlan(int studyPlanId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var result = await _studyPlanService.LeavePublicStudyPlan(userId, studyPlanId);

            if (!result)
            {
                return NotFound("User is not a member of this study plan or the study plan does not exist.");
            }

            return Ok("Successfully left the study plan.");
        }

        /// <summary>
        /// Zmienia właściciela planu nauki
        /// </summary>
        /// <response code="200">Jeżeli właściciel został zmieniony</response>
        /// <response code="400">Jeżeli zmiana właściciela nie powiodła się</response>
        [HttpPost("{studyPlanId}/change-owner")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ChangeOwner(int studyPlanId, [FromBody] int newOwnerId)
        {
            var currentOwnerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var result = await _studyPlanService.ChangeStudyPlanOwner(currentOwnerId, studyPlanId, newOwnerId);

            if (!result)
            {
                return BadRequest("Failed to change owner.");
            }

            return Ok("Ownership has been updated.");
        }
    }
}
