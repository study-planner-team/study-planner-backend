using StudyPlannerAPI.Models.StudyPlans;
using StudyPlannerAPI.Models.Users;
using System.ComponentModel.DataAnnotations;

namespace StudyPlannerAPI.Models.Quizes
{
    public class Quiz
    {
        [Key]
        public int QuizId { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }

        public int CreatedByUserId { get; set; }
        public virtual User CreatedByUser { get; set; }
        public int StudyPlanId { get; set; }
        public virtual StudyPlan StudyPlan { get; set; }

        public virtual ICollection<Question> Questions { get; set; } = new List<Question>();
    }
}
