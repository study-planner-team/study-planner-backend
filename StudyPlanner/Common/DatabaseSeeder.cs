﻿using StudyPlannerAPI.Data;
using StudyPlannerAPI.Models.Quizes;
using StudyPlannerAPI.Models.StudySessions;
using StudyPlannerTests.Common.EntityFactories.Quizzes;
using StudyPlannerTests.Common.EntityFactories.Shared;
using StudyPlannerTests.Common.EntityFactories.Plans;
using StudyPlannerTests.Common.EntityFactories.StudyPlannerTests.Common.EntityFactories;

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

            context.StudyPlans.AddRange(
                StudyPlanFactory.CreateStudyPlan(1, "Plan Owned by User 1", 1, true, false),
                StudyPlanFactory.CreateStudyPlan(2, "Plan Owned by User 2", 2, false, true),
                StudyPlanFactory.CreateStudyPlan(3, "Plan Owned by User 3", 3, false, false)
            );

            context.StudyPlanMembers.AddRange(
                StudyPlanMembersFactory.CreateMembership(2, 1),
                StudyPlanMembersFactory.CreateMembership(1, 2) 
            );

            await context.SaveChangesAsync();
        }

        public static async Task SeedStudyTopics(AppDbContext context)
        {
            context.StudyTopics.AddRange(
                StudyTopicFactory.CreateStudyTopic(1, 1, "Study Topic 1", 4),
                StudyTopicFactory.CreateStudyTopic(2, 1, "Study Topic 2", 6, false),
                StudyTopicFactory.CreateStudyTopic(3, 2, "Study Topic 3", 8)
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

        public static async Task SeedStudySessions(AppDbContext context)
        {
            await SeedStudyTopics(context);

            context.StudySessions.AddRange(
                // Missed session
                StudySessionFactory.CreateStudySession(
                    sessionId: 1,
                    studyPlanId: 1,
                    userId: 1,
                    topicId: 1,
                    date: DateTime.UtcNow.Date.AddDays(-1),
                    startTime: TimeSpan.FromHours(10),
                    endTime: TimeSpan.FromHours(12),
                    status: StudySessionStatus.Missed
                ),

                // In-progress session
                StudySessionFactory.CreateStudySession(
                    sessionId: 2,
                    studyPlanId: 1,
                    userId: 1,
                    topicId: 1,
                    date: DateTime.UtcNow.Date,
                    startTime: TimeSpan.FromHours(DateTime.UtcNow.Hour - 1),
                    endTime: TimeSpan.FromHours(DateTime.UtcNow.Hour + 1),
                    status: StudySessionStatus.InProgress,
                    actualStartTime: TimeSpan.FromHours(DateTime.UtcNow.Hour - 1)
                ),

                // Completed session
                StudySessionFactory.CreateStudySession(
                    sessionId: 3,
                    studyPlanId: 2,
                    userId: 2,
                    topicId: 2,
                    date: DateTime.UtcNow.Date.AddDays(-2),
                    startTime: TimeSpan.FromHours(14),
                    endTime: TimeSpan.FromHours(16),
                    status: StudySessionStatus.Completed,
                    actualDuration: TimeSpan.FromHours(2)
                ),

                // NotStarted session
                StudySessionFactory.CreateStudySession(
                    sessionId: 4,
                    studyPlanId: 2,
                    userId: 2,
                    topicId: 2,
                    date: DateTime.UtcNow.Date.AddDays(1), 
                    startTime: TimeSpan.FromHours(10),
                    endTime: TimeSpan.FromHours(12),
                    status: StudySessionStatus.NotStarted
                )
            );

            await context.SaveChangesAsync();
        }

        public static async Task SeedConflictingStudySessions(AppDbContext context)
        {
            await SeedStudyTopics(context);

            context.StudySessions.AddRange(
                StudySessionFactory.CreateStudySession(
                    sessionId: 5,
                    studyPlanId: 1,
                    userId: 1,
                    topicId: 1,
                    date: DateTime.UtcNow.Date,
                    startTime: TimeSpan.FromHours(8),
                    endTime: TimeSpan.FromHours(10),
                    status: StudySessionStatus.NotStarted
                ),
                StudySessionFactory.CreateStudySession(
                    sessionId: 6,
                    studyPlanId: 1,
                    userId: 1,
                    topicId: 2,
                    date: DateTime.UtcNow.Date.AddDays(1),
                    startTime: TimeSpan.FromHours(9),
                    endTime: TimeSpan.FromHours(11),
                    status: StudySessionStatus.InProgress
                )
            );

            await context.SaveChangesAsync();
        }

        public static async Task SeedQuizzes(AppDbContext context)
        {
            await SeedStudyPlans(context);

            context.Quizzes.AddRange(
                QuizFactory.CreateQuiz(1, 1, 1, "Quiz 1"),
                QuizFactory.CreateQuiz(2, 1, 1, "Quiz 2"),
                QuizFactory.CreateQuiz(3, 2, 2, "Quiz 3", "Advanced Quiz")
            );

            await context.SaveChangesAsync();
        }

        public static async Task SeedQuizAssignments(AppDbContext context)
        {
            await SeedQuizzes(context);

            context.QuizAssignments.AddRange(
                QuizAssignmentFactory.CreateAssignment(1, 1, 1),
                QuizAssignmentFactory.CreateAssignment(2, 2, 2),
                QuizAssignmentFactory.CreateAssignment(3, 3, 1, QuizState.Completed),
                QuizAssignmentFactory.CreateAssignment(4, 2, 1)
            );

            await context.SaveChangesAsync();
        }
    }
}
