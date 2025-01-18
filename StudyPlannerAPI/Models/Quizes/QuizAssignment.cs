using StudyPlannerAPI.Models.Users;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace StudyPlannerAPI.Models.Quizes
{
    public class QuizAssignment
    {
        [Key]
        public int AssignmentId { get; set; }
        public DateTime AssignedOn { get; set; } = DateTime.UtcNow;
        public DateTime? CompletedOn { get; set; } 
        public QuizState State { get; set; } = QuizState.Assigned;
        public int? CorrectAnswers { get; set; } 
        public int? TotalQuestions { get; set; }

        public int QuizId { get; set; }
        public virtual Quiz Quiz { get; set; }

        public int AssignedToUserId { get; set; }
        public virtual User AssignedToUser { get; set; }
    }

    public enum QuizState
    {
        Assigned,
        InProgress,
        Completed
    }
}
