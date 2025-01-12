using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using StudyPlannerAPI.Services.BadgeService;
using StudyPlannerTests.Common;

namespace StudyPlannerTests.Services
{
    public class BadgeServiceTests
    {
        private readonly IMapper _mapper;

        public BadgeServiceTests()
        {
            _mapper = MapperFactory.GetMapper();
        }

        [Fact]
        public async Task GetUserBadges_ShouldReturnBadgesWithEarnedStatus()
        {
            // Arrange
            var context = InMemoryDbContextFactory.Create("TestDb_GetUserBadges");
            await DatabaseSeeder.SeedBadges(context);
            await DatabaseSeeder.SeedUserBadges(context, (1, 1, 1), (2, 1, 3)); // User has badges 1 and 3
            var service = new BadgeService(context, _mapper);

            // Act
            var result = await service.GetUserBadges(1);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(7);
            result.Where(b => b.BadgeId == 1 || b.BadgeId == 3).Should().OnlyContain(b => b.Earned);
            result.Where(b => b.BadgeId != 1 && b.BadgeId != 3).Should().OnlyContain(b => !b.Earned);
        }

        [Fact]
        public async Task AssignBadgesToUser_ShouldAssignFirstStepsBadge_WhenUserHasCreatedStudyPlan()
        {
            // Arrange
            var context = InMemoryDbContextFactory.Create("TestDb_AssignFirstStepsBadge");
            await DatabaseSeeder.SeedBadges(context);
            await DatabaseSeeder.SeedStudyPlans(context); // User has created a study plan
            var service = new BadgeService(context, _mapper);

            // Act
            await service.AssignBadgesToUser(1);

            // Assert
            var userBadges = await context.UserBadges.Where(ub => ub.UserId == 1).ToListAsync();
            userBadges.Should().ContainSingle(b => b.BadgeId == 1); // First Steps badge
        }

        [Fact]
        public async Task AssignBadgesToUser_ShouldNotAssignAnyBadges_WhenUserHasNoActivity()
        {
            // Arrange
            var context = InMemoryDbContextFactory.Create("TestDb_NoActivity");
            await DatabaseSeeder.SeedBadges(context);
            var service = new BadgeService(context, _mapper);

            // Act
            await service.AssignBadgesToUser(1);

            // Assert
            var userBadges = await context.UserBadges.Where(ub => ub.UserId == 1).ToListAsync();
            userBadges.Should().BeEmpty();
        }

        [Fact]
        public async Task AssignBadgesToUser_ShouldAssignQuizGeniusBadge_WhenUserHasCreatedTenQuizzes()
        {
            // Arrange
            var context = InMemoryDbContextFactory.Create("TestDb_QuizGeniusBadge");
            await DatabaseSeeder.SeedStudyPlans(context);
            await DatabaseSeeder.SeedBadges(context);
            await DatabaseSeeder.SeedQuizzesForBadge(context, userId: 1, quizCount: 10);
            var service = new BadgeService(context, _mapper);

            // Act
            await service.AssignBadgesToUser(1);

            // Assert
            var userBadges = await context.UserBadges.Where(ub => ub.UserId == 1).ToListAsync();
            userBadges.Should().ContainSingle(b => b.BadgeId == 2); // Quiz Genius badge
        }

        [Fact]
        public async Task AssignBadgesToUser_ShouldAssignTeamPlayerBadge_WhenUserHasJoinedThreeGroupPlans()
        {
            // Arrange
            var context = InMemoryDbContextFactory.Create("TestDb_TeamPlayerBadge");
            await DatabaseSeeder.SeedStudyPlans(context);
            await DatabaseSeeder.SeedBadges(context);
            await DatabaseSeeder.SeedStudyPlanMembersForBadge(context, userId: 4, groupPlanCount: 3); // User joined 3 group plans
            var service = new BadgeService(context, _mapper);

            // Act
            await service.AssignBadgesToUser(4);

            // Assert
            var userBadges = await context.UserBadges.Where(ub => ub.UserId == 4).ToListAsync();
            userBadges.Should().ContainSingle(b => b.BadgeId == 3); // Ensure "Team Player" badge is assigned
        }



        [Fact]
        public async Task AssignBadgesToUser_ShouldAssignConsistencyMasterBadge_WhenUserHasStudiedSevenConsecutiveDays()
        {
            // Arrange
            var context = InMemoryDbContextFactory.Create("TestDb_ConsistencyMasterBadge");
            await DatabaseSeeder.SeedStudyPlans(context);
            await DatabaseSeeder.SeedStudyTopics(context);
            await DatabaseSeeder.SeedBadges(context);
            await DatabaseSeeder.SeedStudySessionsForBadge(context, userId: 4, consecutiveDays: 7);
            var service = new BadgeService(context, _mapper);

            // Act
            await service.AssignBadgesToUser(4);

            // Assert
            var userBadges = await context.UserBadges.Where(ub => ub.UserId == 4).ToListAsync();
            userBadges.Should().ContainSingle(b => b.BadgeId == 4); // Consistency Master badge
        }

        [Fact]
        public async Task AssignBadgesToUser_ShouldAssignPlannerEnthusiastBadge_WhenUserHasCreatedFiveStudyPlans()
        {
            // Arrange
            var context = InMemoryDbContextFactory.Create("TestDb_PlannerEnthusiastBadge");
            await DatabaseSeeder.SeedUsers(context);
            await DatabaseSeeder.SeedBadges(context);
            await DatabaseSeeder.SeedStudyPlansForBadge(context, userId: 4, planCount: 5);
            var service = new BadgeService(context, _mapper);

            // Act
            await service.AssignBadgesToUser(4);

            // Assert
            var userBadges = await context.UserBadges.Where(ub => ub.UserId == 4).ToListAsync();
            userBadges.Should().ContainSingle(b => b.BadgeId == 5); // Planner Enthusiast badge
        }

        [Fact]
        public async Task AssignBadgesToUser_ShouldAssignTimeKeeperBadge_WhenUserHasStudiedFiftyHours()
        {
            // Arrange
            var context = InMemoryDbContextFactory.Create("TestDb_TimeKeeperBadge");
            await DatabaseSeeder.SeedStudyPlans(context);
            await DatabaseSeeder.SeedStudyTopics(context);
            await DatabaseSeeder.SeedBadges(context);
            await DatabaseSeeder.SeedStudySessionsForBadge(context, userId: 4, consecutiveDays: 25, hoursPerDay: 2); // 50 hours total
            var service = new BadgeService(context, _mapper);

            // Act
            await service.AssignBadgesToUser(4);

            // Assert
            var userBadges = await context.UserBadges.Where(ub => ub.UserId == 4).ToListAsync();
            userBadges.Should().ContainSingle(b => b.BadgeId == 6); // Time Keeper badge
        }

        [Fact]
        public async Task AssignBadgesToUser_ShouldAssignKnowledgeSharerBadge_WhenUserHasSharedAStudyPlan()
        {
            // Arrange
            var context = InMemoryDbContextFactory.Create("TestDb_KnowledgeSharerBadge");
            await DatabaseSeeder.SeedStudyPlans(context); // Seed plans and users
            await DatabaseSeeder.SeedBadges(context);
            var service = new BadgeService(context, _mapper);

            // Act
            await service.AssignBadgesToUser(1); // User 1 has a public study plan

            // Assert
            var userBadges = await context.UserBadges.Where(ub => ub.UserId == 1).ToListAsync();
            userBadges.Should().ContainSingle(b => b.BadgeId == 7); // Knowledge Sharer badge
        }

        [Fact]
        public async Task AssignBadgesToUser_ShouldAssignQuizCreatorBadge_WhenUserHasCreatedTenQuizzes()
        {
            // Arrange
            var context = InMemoryDbContextFactory.Create("TestDb_QuizCreatorBadge");
            await DatabaseSeeder.SeedStudyPlans(context);
            await DatabaseSeeder.SeedBadges(context);
            await DatabaseSeeder.SeedQuizzesForBadge(context, userId: 4, quizCount: 10);
            var service = new BadgeService(context, _mapper);

            // Act
            await service.AssignBadgesToUser(4);

            // Assert
            var userBadges = await context.UserBadges.Where(ub => ub.UserId == 4).ToListAsync();
            userBadges.Should().ContainSingle(b => b.BadgeId == 8); // Quiz Creator badge
        }
    }
}
