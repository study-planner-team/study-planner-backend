using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using StudyPlannerAPI.Data;
using StudyPlannerAPI.Models.StudySessions;
using System;

namespace StudyPlannerAPI.Services.StudySessionsServices
{
    public class StudySessionService : IStudySessionService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        public StudySessionService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<StudySession>?> GenerateAndStoreSchedule(StudySessionDTO scheduleData)
        {
            // Extract TimeSpan from DateTime properties
            var studyStartTime = scheduleData.StudyStartTime.TimeOfDay;
            var studyEndTime = scheduleData.StudyEndTime.TimeOfDay;

            // Validate if there are any preferred study days
            if (scheduleData.PreferredStudyDays == null || !scheduleData.PreferredStudyDays.Any())
            {
                return null; // No preferred study days
            }

            var generatedSessions = new List<StudySession>();
            DateTime currentDate = scheduleData.StartDate.Date;
            var preferredStudyDays = MapDaysToEnum(scheduleData.PreferredStudyDays);

            var topics = await _context.StudyTopics
                .Where(t => scheduleData.TopicIds.Contains(t.TopicId))
                .ToListAsync();

            if (!topics.Any())
            {
                return null; // No topics available for scheduling
            }

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
                var currentStartTime = studyStartTime;
                int sessionsToday = 0;

                if (preferredStudyDays.Contains(currentDate.DayOfWeek))
                {
                    while (sessionsToday < scheduleData.SessionsPerDay &&
                           IsWithinStudyWindow(currentStartTime, studyEndTime, 0.5)) // Smallest unit to check
                    {
                        if (topicIndex >= topics.Count)
                        {
                            break; // No more topics to schedule
                        }

                        // Calculate the maximum available hours in the current day
                        double availableHours = (studyEndTime - currentStartTime).TotalHours;
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
                            currentStartTime = currentStartTime.Add(TimeSpan.FromMinutes(30));
                        }
                    }
                }

                if (sessionsToday == 0 || !IsWithinStudyWindow(currentStartTime, studyEndTime, 0.5))
                {
                    currentDate = MoveToNextPreferredDay(currentDate.AddDays(1), preferredStudyDays);
                }
            }

            // If there are still hours left for topics, return null
            if (topicIndex < topics.Count && remainingHours > 0)
            {
                return null;
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
            if (!preferredStudyDays.Any())
            {
                throw new ArgumentException("Preferred study days cannot be empty.");
            }

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

        public async Task<List<StudySessionResponseDTO>> GetUserStudySessions(int userId)
        {
            var sessions = await _context.StudySessions
                .Include(s => s.StudyTopic)
                .ThenInclude(t => t.StudyMaterials)
                .Where(s => s.UserId == userId && (s.Status == StudySessionStatus.NotStarted || s.Status == StudySessionStatus.InProgress))
                .OrderBy(s => s.Date)
                .ThenBy(s => s.StartTime)
                .ToListAsync();

            var timeZone = TimeZoneInfo.FindSystemTimeZoneById("Europe/Warsaw");


            return _mapper.Map<List<StudySessionResponseDTO>>(sessions);
        }

        public async Task<StudySessionResponseDTO?> GetCurrentSession(int userId)
        {
            var now = DateTime.UtcNow;
            var time = now.TimeOfDay;

            var session = await _context.StudySessions
                .Include(s => s.StudyTopic)
                .ThenInclude(t => t.StudyMaterials)
                .Where(s => s.UserId == userId && s.Date == now.Date && s.StartTime <= time && s.EndTime >= time && (s.Status == StudySessionStatus.NotStarted || s.Status == StudySessionStatus.InProgress))
                .FirstOrDefaultAsync();

            if (session == null) return null;

            return _mapper.Map<StudySessionResponseDTO>(session);
        }
        public async Task<StudySessionResponseDTO?> StartSession(int sessionId)
        {
            var session = await _context.StudySessions
                .Include(s => s.StudyTopic)
                .ThenInclude(t => t.StudyMaterials)
                .FirstOrDefaultAsync(s => s.StudySessionId == sessionId);

            if (session == null || session.Status != StudySessionStatus.NotStarted)
            {
                return null; // Ensure the session is in the correct state to start
            }

            session.ActualStartTime = DateTime.UtcNow.TimeOfDay;
            session.Status = StudySessionStatus.InProgress;

            _context.StudySessions.Update(session);
            await _context.SaveChangesAsync();


            return _mapper.Map<StudySessionResponseDTO>(session);
        }

        public async Task<StudySessionResponseDTO?> EndSession(int sessionId)
        {
            var session = await _context.StudySessions.FindAsync(sessionId);
            if (session == null || session.Status != StudySessionStatus.InProgress)
            {
                return null; // Ensure only in-progress sessions can be ended
            }

            var now = DateTime.UtcNow;

            // Use the actual start time or default to the scheduled start time
            var actualStartTime = session.ActualStartTime ?? session.StartTime;

            // Calculate the actual duration as the difference between now and actual start time
            session.ActualDuration = now.TimeOfDay - actualStartTime;

            // Ensure the duration is not negative
            if (session.ActualDuration < TimeSpan.Zero)
            {
                session.ActualDuration = TimeSpan.Zero;
            }

            // Mark the session as completed
            session.Status = StudySessionStatus.Completed;

            _context.StudySessions.Update(session);
            await _context.SaveChangesAsync();


            return _mapper.Map<StudySessionResponseDTO>(session);
        }

        public async Task MarkExpiredSessionsAsync()
        {
            var now = DateTime.UtcNow;

            var sessions = await _context.StudySessions
                .Where(s => s.Status == StudySessionStatus.NotStarted || s.Status == StudySessionStatus.InProgress)
                .ToListAsync();

            var missedSessions = sessions
                .Where(s => s.Status == StudySessionStatus.NotStarted && s.Date.Add(s.EndTime) < now)
                .ToList();

            var endedSessions = sessions
                .Where(s => s.Status == StudySessionStatus.InProgress && s.Date.Add(s.EndTime) < now)
                .ToList();

            foreach (var session in missedSessions)
            {
                session.Status = StudySessionStatus.Missed;
                session.ActualDuration = TimeSpan.Zero;
            }

            foreach (var session in endedSessions)
            {
                session.Status = StudySessionStatus.Completed;

                if (session.StartTime < session.EndTime)
                {
                    session.ActualDuration = (session.EndTime - session.StartTime).Duration();
                }
                else
                {
                    session.ActualDuration = TimeSpan.Zero;
                }
            }

            _context.StudySessions.UpdateRange(missedSessions);
            _context.StudySessions.UpdateRange(endedSessions);
            await _context.SaveChangesAsync();
        }

        public async Task<List<StudySessionResponseDTO?>> GetCompletedSessions(int userId)
        {
            var sessions = await _context.StudySessions
                .Include(s => s.StudyTopic)
                .Where(s => s.UserId == userId && s.Status == StudySessionStatus.Completed)
                .ToListAsync();

            if (sessions == null) return null;

            return _mapper.Map<List<StudySessionResponseDTO>>(sessions);
        }

        public async Task<StudySessionResponseDTO?> GetNextSession(int userId)
        {
            var now = DateTime.UtcNow;
            var time = now.TimeOfDay;

            var nextSession = await _context.StudySessions
                .Include(s => s.StudyTopic)
                .ThenInclude(t => t.StudyMaterials)
                .Where(s => s.UserId == userId &&
                            (s.Date > now.Date || (s.Date == now.Date && s.StartTime > time)) &&
                            s.Status == StudySessionStatus.NotStarted)
                .OrderBy(s => s.Date)
                .ThenBy(s => s.StartTime)
                .FirstOrDefaultAsync();

            if (nextSession == null) return null;

            return _mapper.Map<StudySessionResponseDTO>(nextSession);
        }


    }
}
