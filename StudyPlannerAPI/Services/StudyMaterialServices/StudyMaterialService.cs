using AutoMapper;
using Microsoft.EntityFrameworkCore;
using StudyPlannerAPI.Data;
using StudyPlannerAPI.Models.StudyMaterials;

namespace StudyPlannerAPI.Services.StudyMaterialServices
{
    public class StudyMaterialService : IStudyMaterialService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public StudyMaterialService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<StudyMaterialResponseDTO>> GetMaterialsByTopicId(int topicId)
        {
            var materials = await _context.StudyMaterials.Where(sm => sm.StudyTopicId == topicId).ToListAsync();

            return _mapper.Map<IEnumerable<StudyMaterialResponseDTO>>(materials);
        }

        public async Task<StudyMaterialResponseDTO> CreateStudyMaterial(int studyTopicId, StudyMaterialDTO materialDTO)
        {
            var material = _mapper.Map<StudyMaterial>(materialDTO);
            material.StudyTopicId = studyTopicId;

            _context.StudyMaterials.Add(material);
            await _context.SaveChangesAsync();

            return _mapper.Map<StudyMaterialResponseDTO>(material);
        }

        public async Task<StudyMaterialResponseDTO?> UpdateStudyMaterial(int id, StudyMaterialDTO materialDTO)
        {
            var material = await _context.StudyMaterials.FirstOrDefaultAsync(sm => sm.StudyMaterialId == id);

            if (material == null)
                return null;

            material.Title = materialDTO.Title;
            material.Link = materialDTO.Link;

            await _context.SaveChangesAsync();

            return _mapper.Map<StudyMaterialResponseDTO>(material);
        }

        public async Task<bool> DeleteStudyMaterial(int id)
        {
            var material = await _context.StudyMaterials.FirstOrDefaultAsync(sm => sm.StudyMaterialId == id);
            
            if (material == null) 
                return false;

            _context.StudyMaterials.Remove(material);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
