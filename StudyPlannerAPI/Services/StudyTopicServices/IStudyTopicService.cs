using StudyPlannerAPI.Models.StudyTopics;

namespace StudyPlannerAPI.Services.StudyTopicServices
{
    public interface IStudyTopicService
    {
        public Task<List<StudyTopicResponseDTO>> GetTopicsForStudyPlan(int studyPlanId);
        public Task<StudyTopic> AddTopicToStudyPlan(int studyPlanId, StudyTopicDTO topicDTO);
    }
}
