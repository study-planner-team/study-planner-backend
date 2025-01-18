using System.ComponentModel.DataAnnotations;

namespace StudyPlannerAPI.Models.Quizes
{
    public class Question
    {
        [Key]
        public int QuestionId { get; set; }
        public string QuestionText { get; set; }

        public int QuizId { get; set; }
        public virtual Quiz Quiz { get; set; }

        public virtual ICollection<QuestionOption> Options { get; set; } = new List<QuestionOption>();
    }
}
