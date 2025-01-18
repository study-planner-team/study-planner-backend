using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudyPlannerTests.Common.EntityFactories
{
    using StudyPlannerAPI.Models.StudySessions;
    using System;

    namespace StudyPlannerTests.Common.EntityFactories
    {
        public static class StudySessionFactory
        {
            public static StudySession CreateStudySession(
                int sessionId,
                int studyPlanId,
                int userId,
                int topicId,
                DateTime date,
                TimeSpan startTime,
                TimeSpan endTime,
                StudySessionStatus status = StudySessionStatus.NotStarted,
                TimeSpan? actualStartTime = null,
                TimeSpan? actualDuration = null)
            {
                return new StudySession
                {
                    StudySessionId = sessionId,
                    StudyPlanId = studyPlanId,
                    UserId = userId,
                    TopicId = topicId,
                    Date = date,
                    Duration = (endTime - startTime).TotalHours,
                    StartTime = startTime,
                    EndTime = endTime,
                    Status = status,
                    ActualStartTime = actualStartTime,
                    ActualDuration = actualDuration
                };
            }
        }
    }

}
