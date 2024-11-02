using StudyPlannerAPI.Models.StudyMaterials;

namespace StudyPlannerAPI.Services.StudyMaterialServices
{
    public interface IStudyMaterialService
    {
        public Task<IEnumerable<StudyMaterialResponseDTO>> GetMaterialsByTopicId(int topicId);
        public Task<StudyMaterialResponseDTO> CreateStudyMaterial(int studyTopicId, StudyMaterialDTO materialDTO);
        public Task<StudyMaterialResponseDTO?> UpdateStudyMaterial(int id, StudyMaterialDTO materialDTO);
        public Task<bool> DeleteStudyMaterial(int id);
    }
}
