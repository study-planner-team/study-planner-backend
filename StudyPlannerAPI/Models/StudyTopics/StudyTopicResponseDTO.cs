using StudyPlannerAPI.Models.StudyMaterials;

namespace StudyPlannerAPI.Models.StudyTopics
{
    public class StudyTopicResponseDTO
    {
        public int TopicId { get; set; }
        public string Title { get; set; }
        public double Hours { get; set; }
        public List<StudyMaterialResponseDTO> StudyMaterials { get; set; }
    }
}
