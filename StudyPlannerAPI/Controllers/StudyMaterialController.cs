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

        [HttpGet]
        public async Task<IActionResult> GetMaterialsByTopicId(int studyTopicId)
        {
            var materials = await _studyMaterialService.GetMaterialsByTopicId(studyTopicId);
            return Ok(materials);
        }

        [HttpPost]
        public async Task<IActionResult> CreateStudyMaterial(int studyTopicId, [FromBody] StudyMaterialDTO materialDTO)
        {
            var validationResult = await _studyMaterialValidator.ValidateAsync(materialDTO);

            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors);

            var material = await _studyMaterialService.CreateStudyMaterial(studyTopicId, materialDTO);

            return Ok(material);
        }

        [HttpPut("{materialId}")]
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

        [HttpDelete("{materialId}")]
        public async Task<IActionResult> DeleteStudyMaterial(int materialId)
        {
            var deleted = await _studyMaterialService.DeleteStudyMaterial(materialId);

            if (!deleted) 
                return NotFound();

            return Ok();
        }
    }
}
