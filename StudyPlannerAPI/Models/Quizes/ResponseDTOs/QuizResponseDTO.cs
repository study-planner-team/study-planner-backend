namespace StudyPlannerAPI.Models.Quizes.ResponseDTOs
{
    public class QuizResponseDTO
    {
        public int QuizId { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public int CreatedByUserId { get; set; }
        public int StudyPlanId { get; set; }
        public ICollection<QuestionResponseDTO> Questions { get; set; } = new List<QuestionResponseDTO>();
    }
}
