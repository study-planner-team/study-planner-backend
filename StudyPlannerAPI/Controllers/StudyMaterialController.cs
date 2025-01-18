using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudyPlannerAPI.Models.StudyMaterials;
using StudyPlannerAPI.Services.StudyMaterialServices;

namespace StudyPlannerAPI.Controllers
{
    [Route("api/topics/{studyTopicId}/materials")]
    [ApiController]
    [Authorize]
    public class StudyMaterialController : ControllerBase
    {
        private readonly IStudyMaterialService _studyMaterialService;
        private readonly IValidator<StudyMaterialDTO> _studyMaterialValidator;

        public StudyMaterialController(IStudyMaterialService studyMaterialService, IValidator<StudyMaterialDTO> studyMaterialValidator)
        {
            _studyMaterialService = studyMaterialService;
            _studyMaterialValidator = studyMaterialValidator;
        }

        /// <summary>
        /// Pobiera materiały dla podanego tematu nauki
        /// </summary>
        /// <response code="200">Zwraca listę materiałów dla podanego tematu</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<StudyMaterialResponseDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMaterialsByTopicId(int studyTopicId)
        {
            var materials = await _studyMaterialService.GetMaterialsByTopicId(studyTopicId);
            return Ok(materials);
        }

        /// <summary>
        /// Tworzy nowy materiał dla tematu nauki
        /// </summary>
        /// <response code="200">Zwraca dane utworzonego materiału</response>
        /// <response code="400">Jeżeli dane wejściowe są nieprawidłowe</response>
        [HttpPost]
        [ProducesResponseType(typeof(StudyMaterialResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateStudyMaterial(int studyTopicId, [FromBody] StudyMaterialDTO materialDTO)
        {
            var validationResult = await _studyMaterialValidator.ValidateAsync(materialDTO);

            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors);

            var material = await _studyMaterialService.CreateStudyMaterial(studyTopicId, materialDTO);

            return Ok(material);
        }

        /// <summary>
        /// Aktualizuje materiał nauki
        /// </summary>
        /// <response code="200">Zwraca dane zaktualizowanego materiału</response>
        /// <response code="400">Jeżeli dane wejściowe są nieprawidłowe</response>
        /// <response code="404">Jeżeli materiał nie istnieje</response>
        [HttpPut("{materialId}")]
        [ProducesResponseType(typeof(StudyMaterialResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateStudyMaterial(int materialId, [FromBody] StudyMaterialDTO materialDTO)
        {
            var validationResult = await _studyMaterialValidator.ValidateAsync(materialDTO);

            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors);

            var updatedMaterial = await _studyMaterialService.UpdateStudyMaterial(materialId, materialDTO);

            if (updatedMaterial == null)
                return NotFound();

            return Ok(updatedMaterial);
        }

        /// <summary>
        /// Usuwa materiał nauki
        /// </summary>
        /// <response code="200">Jeżeli materiał został usunięty</response>
        /// <response code="404">Jeżeli materiał nie istnieje</response>
        [HttpDelete("{materialId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteStudyMaterial(int materialId)
        {
            var deleted = await _studyMaterialService.DeleteStudyMaterial(materialId);

            if (!deleted)
                return NotFound();

            return Ok();
        }
    }
}
