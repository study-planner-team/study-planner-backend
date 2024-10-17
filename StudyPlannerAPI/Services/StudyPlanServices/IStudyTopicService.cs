using StudyPlannerAPI.Models.StudyPlans;

namespace StudyPlannerAPI.Services.StudyPlanServices
{
    public interface IStudyTopicService
    {
        public Task<List<StudyTopic>> GetTopicsForStudyPlan(int studyPlanId);
        public Task<StudyTopic> AddTopicToStudyPlan(int studyPlanId, StudyTopicDTO topicDTO);
    }
}
