using System.ComponentModel.DataAnnotations;
using StudyPlannerAPI.Models.StudyMaterials;
using StudyPlannerAPI.Models.StudyPlans;

namespace StudyPlannerAPI.Models.StudyTopics
{
    public class StudyTopic
    {
        [Key]
        public int TopicId { get; set; }
        public required string Title { get; set; }
        public required double Hours { get; set; }
        public int StudyPlanId { get; set; }
        public StudyPlan StudyPlan { get; set; }
        public virtual ICollection<StudyMaterial> StudyMaterials { get; set; }
    }
}