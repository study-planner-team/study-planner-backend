using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using StudyPlannerAPI.Models.StudyTopics;

namespace StudyPlannerAPI.Models.StudyMaterials
{
    public class StudyMaterial
    {
        [Key]
        public int StudyMaterialId { get; set; }

        public required string Title { get; set; }

        public required string Link { get; set; }

        public int StudyTopicId { get; set; }

        public StudyTopic StudyTopic { get; set; }
    }
}
