using Microsoft.EntityFrameworkCore;
using StudyPlannerAPI.Data;
using StudyPlannerAPI.Models.StudySessions;

namespace StudyPlannerAPI.Services.StudySessionsServices
{
    public class StudySessionService : IStudySessionService
    {
        private readonly AppDbContext _context;
        public StudySessionService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<StudySession>> GetUserStudySessions(int userId)
        {
            return await _context.StudySessions
                .Include(s => s.StudyTopic)
                .Where(s => s.UserId == userId)
                .OrderBy(s => s.Date)
                .ThenBy(s => s.StartTime)
                .ToListAsync();
        }

        public async Task<List<StudySession>?> GenerateAndStoreSchedule(StudySessionDTO scheduleData)
        {
            var generatedSessions = new List<StudySession>();
            DateTime currentDate = scheduleData.StartDate;
            var preferredStudyDays = MapDaysToEnum(scheduleData.PreferredStudyDays);

            var topics = await _context.StudyTopics
                .Where(t => scheduleData.TopicIds.Contains(t.TopicId))
                .ToListAsync();
            topics = topics.OrderBy(t => scheduleData.TopicIds.IndexOf(t.TopicId)).ToList();

            var existingSessions = await _context.StudySessions
                .Where(s => s.UserId == scheduleData.UserId &&
                            s.Date >= scheduleData.StartDate &&
                            s.Date <= scheduleData.EndDate)
                .ToListAsync();

            int topicIndex = 0;
            double remainingHours = topics[topicIndex].Hours;

            while (currentDate <= scheduleData.EndDate)
            {
                var currentStartTime = scheduleData.StudyStartTime;
                int sessionsToday = 0;

                if (preferredStudyDays.Contains(currentDate.DayOfWeek))
                {
                    while (sessionsToday < scheduleData.SessionsPerDay &&
                           IsWithinStudyWindow(currentStartTime, scheduleData.StudyEndTime, 0.5)) // Smallest unit to check
                    {
                        if (topicIndex >= topics.Count)
                        {
                            break; // No more topics to schedule
                        }

                        // Calculate the maximum available hours in the current day
                        double availableHours = (scheduleData.StudyEndTime - currentStartTime).TotalHours;
                        double sessionHours = Math.Min(remainingHours, Math.Min(scheduleData.SessionLength, availableHours));

                        // If we have time for a session and it doesn't conflict with existing sessions
                        if (sessionHours > 0 && !IsTimeSlotTaken(currentDate, currentStartTime, sessionHours, existingSessions))
                        {
                            var newSession = new StudySession
                            {
                                Date = currentDate,
                                Duration = sessionHours * 60, // Convert hours to minutes
                                StudyPlanId = scheduleData.StudyPlanId,
                                UserId = scheduleData.UserId,
                                TopicId = topics[topicIndex].TopicId,
                                StartTime = currentStartTime,
                                EndTime = currentStartTime.Add(TimeSpan.FromHours(sessionHours))
                            };

                            generatedSessions.Add(newSession);
                            existingSessions.Add(newSession); // Add the new session to existingSessions

                            remainingHours -= sessionHours;
                            sessionsToday++;
                            currentStartTime = currentStartTime.Add(TimeSpan.FromHours(sessionHours));

                            if (remainingHours <= 0)
                            {
                                topicIndex++;
                                if (topicIndex < topics.Count)
                                {
                                    remainingHours = topics[topicIndex].Hours;
                                }
                            }
                        }
                        else
                        {
                            currentStartTime = currentStartTime.Add(TimeSpan.FromMinutes(30)); // Increment by 30 mins to check next slot
                        }
                    }
                }

                if (sessionsToday == 0 || !IsWithinStudyWindow(currentStartTime, scheduleData.StudyEndTime, 0.5))
                {
                    currentDate = MoveToNextPreferredDay(currentDate.AddDays(1), preferredStudyDays);
                }
            }

            if (generatedSessions.Count == 0)
            {
                return null; // Return null if no sessions could be generated
            }

            // Save all generated sessions to the database
            await _context.StudySessions.AddRangeAsync(generatedSessions);
            await _context.SaveChangesAsync();

            return generatedSessions;
        }

        private List<DayOfWeek> MapDaysToEnum(List<string> preferredStudyDays)
        {
            var dayMapping = new Dictionary<string, DayOfWeek>
            {
                { "Sunday", DayOfWeek.Sunday },
                { "Monday", DayOfWeek.Monday },
                { "Tuesday", DayOfWeek.Tuesday },
                { "Wednesday", DayOfWeek.Wednesday },
                { "Thursday", DayOfWeek.Thursday },
                { "Friday", DayOfWeek.Friday },
                { "Saturday", DayOfWeek.Saturday }
            };

            return preferredStudyDays.Select(day => dayMapping[day]).ToList();
        }

        private bool IsWithinStudyWindow(TimeSpan startTime, TimeSpan endTime, double sessionDurationInHours)
        {
            // Adds the session duration to the start time and checks if it remains within the study end time.
            return startTime.Add(TimeSpan.FromHours(sessionDurationInHours)) <= endTime;
        }

        private bool IsTimeSlotTaken(DateTime date, TimeSpan startTime, double sessionDuration, List<StudySession> sessions)
        {
            if (sessions == null)
            {
                return false;
            }

            var sessionEndTime = startTime.Add(TimeSpan.FromHours(sessionDuration));

            // Checks if any existing session on the same date overlaps with the proposed time slot.
            return sessions.Any(session =>
                session.Date == date && (
                    (startTime >= session.StartTime && startTime < session.EndTime) || // Overlaps at the start
                    (sessionEndTime > session.StartTime && sessionEndTime <= session.EndTime) || // Overlaps at the end
                    (startTime <= session.StartTime && sessionEndTime >= session.EndTime) // Completely overlaps an existing session
                )
            );
        }

        private DateTime MoveToNextPreferredDay(DateTime currentDate, List<DayOfWeek> preferredStudyDays)
        {
            // Increment the date by one day until it matches a preferred study day.
            while (!preferredStudyDays.Contains(currentDate.DayOfWeek))
            {
                currentDate = currentDate.AddDays(1);
            }
            return currentDate;
        }

        public async Task<bool> DeleteStudySession(int sessionId)
        {
            var session = await _context.StudySessions.FirstOrDefaultAsync(s => s.StudySessionId == sessionId);

            if (session == null)
                return false;

            _context.StudySessions.Remove(session);
            await _context.SaveChangesAsync();

            return true;
        }

    }
}
