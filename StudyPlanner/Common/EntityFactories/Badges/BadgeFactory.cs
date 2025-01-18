using StudyPlannerAPI.Models.Badges;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudyPlannerTests.Common.EntityFactories.Badges
{
    public static class BadgeFactory
    {
        public static Badge CreateBadge(int badgeId, string name, string description, string iconPath)
        {
            return new Badge
            {
                BadgeId = badgeId,
                Title = name,
                Description = description,
                IconPath = iconPath
            };
        }
    }
}
