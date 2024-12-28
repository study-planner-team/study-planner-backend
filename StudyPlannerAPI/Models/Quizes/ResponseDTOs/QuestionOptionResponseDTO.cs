namespace StudyPlannerAPI.Models.Quizes.ResponseDTOs
{
    public class QuestionOptionResponseDTO
    {
        public int OptionId { get; set; }
        public string OptionText { get; set; }
        public bool IsCorrect { get; set; }
    }
}
