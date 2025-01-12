using StudyPlannerAPI.Models.Badges;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudyPlannerTests.Common.EntityFactories.Badges
{
    public static class UserBadgeFactory
    {
        public static UserBadge CreateUserBadge(int userBadgeId, int userId, int badgeId)
        {
            return new UserBadge
            {
                UserBadgeId = userBadgeId,
                UserId = userId,
                BadgeId = badgeId,
                EarnedDate = DateTime.UtcNow
            };
        }
    }
}
