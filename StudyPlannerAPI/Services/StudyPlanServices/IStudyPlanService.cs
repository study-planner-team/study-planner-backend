using StudyPlannerAPI.Models.StudyPlans;

namespace StudyPlannerAPI.Services.StudyPlanServices
{
    public interface IStudyPlanService
    {
        public Task<StudyPlanResponseDTO> CreateStudyPlan(int userId, StudyPlanDTO studyPlanDTO);
        public Task<StudyPlanResponseDTO?> GetStudyPlanById(int planId);
        public Task<IEnumerable<StudyPlanResponseDTO>> GetStudyPlansForUser(int userId);
        public Task<StudyPlanResponseDTO?> UpdateStudyPlan(int planId, StudyPlanDTO studyPlanDTO);
        public Task<bool> DeleteStudyPlan(int planId);
        public Task<StudyPlanResponseDTO> ArchiveStudyPlan(int planId);
        public Task<StudyPlanResponseDTO?> UnarchiveStudyPlan(int planId);
        public Task<IEnumerable<StudyPlanResponseDTO>> GetArchivedStudyPlansForUser(int userId);
        public Task<IEnumerable<StudyPlanResponseDTO>> GetPublicStudyPlans();
    }
}
