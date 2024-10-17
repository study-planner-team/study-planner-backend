using AutoMapper;
using Microsoft.EntityFrameworkCore;
using StudyPlannerAPI.Data;
using StudyPlannerAPI.Models.StudyPlans;

namespace StudyPlannerAPI.Services.StudyPlanServices
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

        public async Task<List<StudyTopic>> GetTopicsForStudyPlan(int studyPlanId)
        {
            return await _context.StudyTopics.Where(st => st.StudyPlanId == studyPlanId).ToListAsync();
        }

        public async Task<StudyTopic> AddTopicToStudyPlan(int studyPlanId, StudyTopicDTO topicDTO)
        {
            var topic = _mapper.Map<StudyTopic>(topicDTO);

            topic.StudyPlanId = studyPlanId;

            _context.StudyTopics.Add(topic);
            await _context.SaveChangesAsync();
            return topic;
        }

    }
}