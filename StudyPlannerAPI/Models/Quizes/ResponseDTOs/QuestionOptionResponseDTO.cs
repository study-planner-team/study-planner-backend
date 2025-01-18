using System.Text.Json.Serialization;

namespace StudyPlannerAPI.Models.Quizes.ResponseDTOs
{
    public class QuestionOptionResponseDTO
    {
        public int OptionId { get; set; }
        public string OptionText { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? IsCorrect { get; set; }
    }
}
