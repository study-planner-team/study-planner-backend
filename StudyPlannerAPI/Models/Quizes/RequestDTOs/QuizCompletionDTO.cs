namespace StudyPlannerAPI.Models.Quizes.RequestDTOs
{
    public class QuizCompletionDTO
    {
        // public int CorrectAnswers { get; set; }
        // public int TotalQuestions { get; set; }

        public ICollection<UserAnswerDTO> Answers { get; set; } = new List<UserAnswerDTO>();
    }

    public class UserAnswerDTO
    {
        public int QuestionId { get; set; }
        public int SelectedOptionId { get; set; }
    }

}
