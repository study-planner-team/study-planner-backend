namespace StudyPlannerAPI.Models.Quizes.RequestDTOs
{
    public class QuizRequestDTO
    {
        public string Title { get; set; }
        public string? Description { get; set; }
        public ICollection<QuestionRequestDTO> Questions { get; set; } = new List<QuestionRequestDTO>();
    }
}
