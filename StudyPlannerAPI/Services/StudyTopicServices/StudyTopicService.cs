using AutoMapper;
using Microsoft.EntityFrameworkCore;
using StudyPlannerAPI.Data;
using StudyPlannerAPI.Models.StudyMaterials;
using StudyPlannerAPI.Models.StudyTopics;

namespace StudyPlannerAPI.Services.StudyTopicServices
{
    public class StudyTopicService : IStudyTopicService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public StudyTopicService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<StudyTopicResponseDTO>> GetTopicsForStudyPlan(int studyPlanId)
        {
            var studyTopics = await _context.StudyTopics
                .Where(st => st.StudyPlanId == studyPlanId)
                .Include(st => st.StudyMaterials) // Include related StudyMaterials
                .ToListAsync();

            var studyTopicDTOs = _mapper.Map<List<StudyTopicResponseDTO>>(studyTopics);

            return studyTopicDTOs;
        }

        public async Task<StudyTopic> AddTopicToStudyPlan(int studyPlanId, StudyTopicDTO topicDTO)
        {
            var topic = _mapper.Map<StudyTopic>(topicDTO);

            topic.StudyPlanId = studyPlanId;

            _context.StudyTopics.Add(topic);
            await _context.SaveChangesAsync();
            return topic;
        }

        public async Task<bool> DeleteStudyTopic(int topicId)
        {
            var topic = await _context.StudyTopics.FirstOrDefaultAsync(st => st.TopicId == topicId);

            if (topic == null)
                return false;

            _context.StudyTopics.Remove(topic);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}