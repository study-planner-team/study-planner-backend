using StudyPlannerAPI.Models.StudyMaterials;

namespace StudyPlannerAPI.Models.StudyTopics
{
    public class StudyTopicDTO
    {
        public required string Title { get; set; }
        public required double Hours { get; set; }
    }
}