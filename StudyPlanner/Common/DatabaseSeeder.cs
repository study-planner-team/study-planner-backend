using StudyPlannerAPI.Data;
using StudyPlannerTests.Common.EntityFactories;

namespace StudyPlannerTests.Common
{
    public static class DatabaseSeeder
    {
        public static async Task SeedUsers(AppDbContext context)
        {
            context.Users.AddRange(
                UserFactory.CreateUser(1, "OwnerUser", "Test123!", "owner@test.com", "valid-refresh-token", DateTime.UtcNow.AddDays(7)),
                UserFactory.CreateUser(2, "MemberUser", "Test123!", "member@test.com", "expired-refresh-token", DateTime.UtcNow.AddDays(-1)),
                UserFactory.CreateUser(3, "NoMembershipUser", "Test123!", "no_membership@test.com", "valid-refresh-token", DateTime.UtcNow.AddDays(7))
            );

            await context.SaveChangesAsync();
        }

        public static async Task SeedStudyPlans(AppDbContext context)
        {
            await SeedUsers(context);

            // Add study plans
            context.StudyPlans.AddRange(
                StudyPlanFactory.CreateStudyPlan(1, "Plan Owned by User 1", 1, true, false),
                StudyPlanFactory.CreateStudyPlan(2, "Plan Owned by User 2", 2, false, true),
                StudyPlanFactory.CreateStudyPlan(3, "Plan Owned by User 3", 3, false, false)
            );

            // Add memberships
            context.StudyPlanMembers.AddRange(
                StudyPlanMembersFactory.CreateMembership(2, 1),
                StudyPlanMembersFactory.CreateMembership(1, 2) 
            );

            await context.SaveChangesAsync();
        }

        public static async Task SeedStudyTopics(AppDbContext context)
        {
            context.StudyTopics.AddRange(
                StudyTopicFactory.CreateStudyTopic(1, 1, "Study Topic 1"),
                StudyTopicFactory.CreateStudyTopic(2, 1, "Study Topic 2", false),
                StudyTopicFactory.CreateStudyTopic(3, 2, "Study Topic 3")
            );

            await context.SaveChangesAsync();
        }

        public static async Task SeedStudyMaterials(AppDbContext context)
        {
            context.StudyMaterials.AddRange(
                StudyMaterialFactory.CreateStudyMaterial(1, 1, "Material 1", "http://example.com/material1"),
                StudyMaterialFactory.CreateStudyMaterial(2, 1, "Material 2", "http://example.com/material2"),
                StudyMaterialFactory.CreateStudyMaterial(3, 2, "Material 3", "http://example.com/material3")
            );

            await context.SaveChangesAsync();
        }
    }
}
