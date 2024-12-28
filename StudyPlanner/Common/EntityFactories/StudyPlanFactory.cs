using StudyPlannerAPI.Models.StudyPlans;
using StudyPlannerAPI.Models.Users;

namespace StudyPlannerTests.Common.EntityFactories
{
    public static class StudyPlanFactory
    {
        public static StudyPlan CreateStudyPlan(int planId, string title, int userId, bool isPublic,  bool isArchived)
        {
            return new StudyPlan
            {
                StudyPlanId = planId,
                Title = title,
                Description = "Test Study Plan Description",
                IsPublic = isPublic,
                IsArchived = isArchived,
                UserId = userId
            };
        }
    }
}
