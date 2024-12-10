using StudyPlannerAPI.Models.StudySessions;

namespace StudyPlannerAPI.Services.StudySessionsServices
{
    public interface IStudySessionService
    {
        public Task<List<StudySession>?> GenerateAndStoreSchedule(StudySessionDTO scheduleData);
        public Task<List<StudySessionResponseDTO>> GetUserStudySessions(int userId);
        public Task<bool> DeleteStudySession(int sessionId);
        public Task<StudySessionResponseDTO?> GetCurrentSession(int userId);
        public Task<StudySessionResponseDTO?> StartSession(int sessionId);
        public Task<StudySessionResponseDTO?> EndSession(int sessionId);
        public Task<StudySessionResponseDTO?> GetCompletedSessions(int userId);
        public Task<StudySessionResponseDTO?> GetNextSession(int userId);
        public Task MarkExpiredSessionsAsync();
    }
}
