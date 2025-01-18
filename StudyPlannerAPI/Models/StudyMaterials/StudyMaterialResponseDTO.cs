namespace StudyPlannerAPI.Models.StudyMaterials
{
    public class StudyMaterialResponseDTO
    {
        public required int StudyMaterialId { get; set; }
        public required string Title { get; set; }
        public required string Link { get; set; }
    }
}
