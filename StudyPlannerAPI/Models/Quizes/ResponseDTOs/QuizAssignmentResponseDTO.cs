namespace StudyPlannerAPI.Models.Quizes.ResponseDTOs
{
    public class QuizAssignmentResponseDTO
    {
        public int AssignmentId { get; set; }
        public DateTime AssignedOn { get; set; }
        public string State { get; set; }
        public int? CorrectAnswers { get; set; }
        public int? TotalQuestions { get; set; }
        public QuizResponseDTO Quiz { get; set; }
    }
}
