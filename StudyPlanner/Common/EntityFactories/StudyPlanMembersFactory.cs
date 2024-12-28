using StudyPlannerAPI.Models.StudyPlans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudyPlannerTests.Common.EntityFactories
{
    public static class StudyPlanMembersFactory
    {
        public static StudyPlanMembers CreateMembership(int userId, int studyPlanId)
        {
            return new StudyPlanMembers
            {
                UserId = userId,
                StudyPlanId = studyPlanId,
                JoinedDate = DateTime.UtcNow
            };
        }
    }
}
