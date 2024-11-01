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

            // Retrieve existing sessions for the user within the specified date range
            var existingSessions = await _context.StudySessions
                .Where(s => s.UserId == scheduleData.UserId &&
                            s.Date >= scheduleData.StartDate &&
                            s.Date <= scheduleData.EndDate)
                .ToListAsync();

            foreach (var topic in scheduleData.Topics)
            {
                double remainingHours = topic.Hours;

                while (remainingHours > 0)
                {
                    if (currentDate > scheduleData.EndDate)
                    {
                        return null; // Return null if we exceed the end date
                    }

                    var currentStartTime = scheduleData.StudyStartTime;
                    int sessionsToday = 0;

                    if (preferredStudyDays.Contains(currentDate.DayOfWeek))
                    {
                        while (sessionsToday < scheduleData.SessionsPerDay &&
                               IsWithinStudyWindow(currentStartTime, scheduleData.StudyEndTime, scheduleData.SessionLength) &&
                               remainingHours > 0)
                        {
                            double sessionHours = Math.Min(remainingHours, scheduleData.SessionLength);

                            // Check for conflicts directly within existingSessions
                            if (!IsTimeSlotTaken(currentDate, currentStartTime, sessionHours, existingSessions))
                            {
                                var newSession = new StudySession
                                {
                                    Date = currentDate,
                                    Duration = sessionHours * 60,
                                    TopicTitle = topic.Title,
                                    StudyPlanId = scheduleData.StudyPlanId,
                                    UserId = scheduleData.UserId,
                                    StartTime = currentStartTime,
                                    EndTime = currentStartTime.Add(TimeSpan.FromHours(sessionHours))
                                };

                                generatedSessions.Add(newSession);
                                existingSessions.Add(newSession); // Add the new session to existingSessions

                                remainingHours -= sessionHours;
                                sessionsToday++;
                                currentStartTime = currentStartTime.Add(TimeSpan.FromHours(sessionHours));
                            }
                            else
                            {
                                break;
                            }
                        }
                    }

                    currentDate = MoveToNextPreferredDay(currentDate.AddDays(1), preferredStudyDays);
                }
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
            return sessions.Any(session => session.Date == date &&
                ((startTime >= session.StartTime && startTime < session.EndTime) ||
                (sessionEndTime > session.StartTime && sessionEndTime <= session.EndTime)));
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

    }
}
