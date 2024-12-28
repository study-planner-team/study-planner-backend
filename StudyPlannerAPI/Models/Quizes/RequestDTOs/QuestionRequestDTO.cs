namespace StudyPlannerAPI.Models.Quizes.RequestDTOs
{
    public class QuestionRequestDTO
    {
        public string QuestionText { get; set; }
        public ICollection<QuestionOptionRequestDTO> Options { get; set; } = new List<QuestionOptionRequestDTO>();
    }
}
