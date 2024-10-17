using StudyPlannerAPI.Models.StudySessions;

namespace StudyPlannerAPI.Services.StudySessionsServices
{
    public class StudySessionService : IStudySessionService
    {
        public Task<List<StudySession>> GenerateAndStoreSchedule(StudySessionDTO scheduleData)
        {
            return Task.FromResult(new List<StudySession>());
        }
    }
}
