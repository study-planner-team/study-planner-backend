using StudyPlannerAPI.Models.StudySessions;

namespace StudyPlannerAPI.Services.StudySessionsServices
{
    public interface IStudySessionService
    {
        public Task<List<StudySession>?> GenerateAndStoreSchedule(StudySessionDTO scheduleData);
        public Task<List<StudySession>> GetUserStudySessions(int userId);
    }
}
