using StudyPlannerAPI.Models.StudySessions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudyPlannerTests.Common.EntityFactories.StudySessions
{
    public static class StudySessionDTOFactory
    {
        public static StudySessionDTO CreateSchedule(
            int studyPlanId,
            int userId,
            DateTime startDate,
            DateTime endDate,
            int sessionsPerDay,
            int sessionLength,
            DateTime studyStartTime,
            DateTime studyEndTime,
            List<string> preferredDays,
            List<int> topicIds)
        {
            return new StudySessionDTO
            {
                StudyPlanId = studyPlanId,
                UserId = userId,
                StartDate = startDate,
                EndDate = endDate,
                SessionsPerDay = sessionsPerDay,
                SessionLength = sessionLength,
                StudyStartTime = studyStartTime,
                StudyEndTime = studyEndTime,
                PreferredStudyDays = preferredDays,
                TopicIds = topicIds
            };
        }
    }
}
