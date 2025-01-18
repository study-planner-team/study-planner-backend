using System.ComponentModel.DataAnnotations;

namespace StudyPlannerAPI.Models.Quizes
{
    public class QuestionOption
    {
        [Key]
        public int OptionId { get; set; }
        public string OptionText { get; set; }
        public bool IsCorrect { get; set; }

        public int QuestionId { get; set; }
        public virtual Question Question { get; set; }
    }
}
