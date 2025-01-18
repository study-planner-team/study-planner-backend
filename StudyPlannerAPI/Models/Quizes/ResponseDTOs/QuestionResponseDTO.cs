namespace StudyPlannerAPI.Models.Quizes.ResponseDTOs
{
    public class QuestionResponseDTO
    {
        public int QuestionId { get; set; }
        public string QuestionText { get; set; }
        public ICollection<QuestionOptionResponseDTO> Options { get; set; } = new List<QuestionOptionResponseDTO>();
    }
}
