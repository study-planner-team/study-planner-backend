using AutoMapper;
using FluentAssertions;
using StudyPlannerAPI.Models.StudySessions;
using StudyPlannerAPI.Services.StudySessionsServices;
using StudyPlannerTests.Common;
using StudyPlannerTests.Common.EntityFactories.StudySessions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudyPlannerTests.Services
{
    public class StudySessionServiceTests
    {
        private readonly IMapper _mapper;
        public StudySessionServiceTests()
        {
            _mapper = MapperFactory.GetMapper();
        }

        [Fact]
        public async Task DeleteStudySession_ShouldReturnTrue_WhenSessionExists()
        {
            // Arrange
            var context = InMemoryDbContextFactory.Create("TestDb_DeleteStudySession_Exists");
            await DatabaseSeeder.SeedStudySessions(context);
            var service = new StudySessionService(context, _mapper);

            // Act
            var result = await service.DeleteStudySession(1);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task DeleteStudySession_ShouldReturnFalse_WhenSessionDoesNotExist()
        {
            // Arrange
            var context = InMemoryDbContextFactory.Create("TestDb_DeleteStudySession_NotExists");
            await DatabaseSeeder.SeedStudySessions(context);
            var service = new StudySessionService(context, _mapper);

            // Act
            var result = await service.DeleteStudySession(99);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task GetUserStudySessions_ShouldReturnSessions_WhenUserHasValidSessions()
        {
            // Arrange
            var context = InMemoryDbContextFactory.Create("TestDb_GetUserStudySessions");
            await DatabaseSeeder.SeedStudySessions(context);
            var service = new StudySessionService(context, _mapper);

            // Act
            var result = await service.GetUserStudySessions(1);

            // Assert
            result.Should().NotBeEmpty();
            result.Should().AllBeOfType<StudySessionResponseDTO>();
            result.Should().OnlyContain(s => s.Status == StudySessionStatus.NotStarted || s.Status == StudySessionStatus.InProgress);
        }

        [Fact]
        public async Task GetUserStudySessions_ShouldReturnEmpty_WhenUserHasNoSessions()
        {
            // Arrange
            var context = InMemoryDbContextFactory.Create("TestDb_GetUserStudySessions_Empty");
            await DatabaseSeeder.SeedUsers(context);
            await DatabaseSeeder.SeedStudySessions(context);
            var service = new StudySessionService(context, _mapper);

            // Act
            var result = await service.GetUserStudySessions(3);

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetCurrentSession_ShouldReturnSession_WhenSessionExists()
        {
            // Arrange
            var context = InMemoryDbContextFactory.Create("TestDb_GetCurrentSession");
            await DatabaseSeeder.SeedStudySessions(context);
            var service = new StudySessionService(context, _mapper);


            // Act
            var result = await service.GetCurrentSession(1);

            // Assert
            result.Should().NotBeNull();
            result!.StudySessionId.Should().Be(2);
        }

        [Fact]
        public async Task GetCurrentSession_ShouldReturnNull_WhenNoSessionExists()
        {
            // Arrange
            var context = InMemoryDbContextFactory.Create("TestDb_GetCurrentSession_NoMatch");
            await DatabaseSeeder.SeedStudySessions(context);
            var service = new StudySessionService(context, _mapper);

            // Act
            var result = await service.GetCurrentSession(99);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task StartSession_ShouldStartSession_WhenSessionExistsAndNotStarted()
        {
            // Arrange
            var context = InMemoryDbContextFactory.Create("TestDb_StartSession");
            await DatabaseSeeder.SeedStudySessions(context);
            var service = new StudySessionService(context, _mapper);

            // Act
            var result = await service.StartSession(4);

            // Assert
            result.Should().NotBeNull();
            result!.Status.Should().Be(StudySessionStatus.InProgress);
            result.Should().BeOfType<StudySessionResponseDTO>();
        }

        [Fact]
        public async Task StartSession_ShouldReturnNull_WhenSessionDoesNotExist()
        {
            // Arrange
            var context = InMemoryDbContextFactory.Create("TestDb_StartSession_NotExists");
            await DatabaseSeeder.SeedStudySessions(context);
            var service = new StudySessionService(context, _mapper);

            // Act
            var result = await service.StartSession(99);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task EndSession_ShouldEndSession_WhenSessionIsInProgress()
        {
            // Arrange
            var context = InMemoryDbContextFactory.Create("TestDb_EndSession");
            await DatabaseSeeder.SeedStudySessions(context);
            var service = new StudySessionService(context, _mapper);

            // Act
            var result = await service.EndSession(2);

            // Assert
            result.Should().NotBeNull();
            result!.Status.Should().Be(StudySessionStatus.Completed);
            result.ActualDuration.Should().NotBeNull();
            result.Should().BeOfType<StudySessionResponseDTO>();
        }

        [Fact]
        public async Task EndSession_ShouldReturnNull_WhenSessionIsNotInProgress()
        {
            // Arrange
            var context = InMemoryDbContextFactory.Create("TestDb_EndSession_NotInProgress");
            await DatabaseSeeder.SeedStudySessions(context);
            var service = new StudySessionService(context, _mapper);

            // Act
            var result = await service.EndSession(4);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetCompletedSessions_ShouldReturnCompletedSessions_WhenUserHasCompletedSessions()
        {
            // Arrange
            var context = InMemoryDbContextFactory.Create("TestDb_GetCompletedSessions");
            await DatabaseSeeder.SeedStudySessions(context);
            var service = new StudySessionService(context, _mapper);

            // Act
            var result = await service.GetCompletedSessions(2);

            // Assert
            result.Should().NotBeNull();
            result.Should().NotBeEmpty();
            result.Should().AllBeOfType<StudySessionResponseDTO>();
            result.Should().OnlyContain(s => s.Status == StudySessionStatus.Completed);
            result.Should().ContainSingle(s => s.StudySessionId == 3);
        }

        [Fact]
        public async Task GetCompletedSessions_ShouldReturnEmpty_WhenUserHasNoCompletedSessions()
        {
            // Arrange
            var context = InMemoryDbContextFactory.Create("TestDb_GetCompletedSessions_Empty");
            await DatabaseSeeder.SeedStudySessions(context);
            var service = new StudySessionService(context, _mapper);

            // Act
            var result = await service.GetCompletedSessions(1);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetNextSession_ShouldReturnNextSession_WhenUserHasUpcomingSessions()
        {
            // Arrange
            var context = InMemoryDbContextFactory.Create("TestDb_GetNextSession");
            await DatabaseSeeder.SeedUsers(context);
            await DatabaseSeeder.SeedStudySessions(context);
            var service = new StudySessionService(context, _mapper);

            // Act
            var result = await service.GetNextSession(2);

            // Assert
            result.Should().NotBeNull();
            result!.StudySessionId.Should().Be(4);
            result.Status.Should().Be(StudySessionStatus.NotStarted);
            result.Should().BeOfType<StudySessionResponseDTO>();
        }

        [Fact]
        public async Task GetNextSession_ShouldReturnNull_WhenUserHasNoUpcomingSessions()
        {
            // Arrange
            var context = InMemoryDbContextFactory.Create("TestDb_GetNextSession_NoUpcoming");
            await DatabaseSeeder.SeedStudySessions(context);
            var service = new StudySessionService(context, _mapper);

            // Act
            var result = await service.GetNextSession(3);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GenerateAndStoreSchedule_ShouldGenerateSessions_WhenNoConflicts()
        {
            // Arrange
            var context = InMemoryDbContextFactory.Create("TestDb_NoConflicts");
            await DatabaseSeeder.SeedStudyTopics(context);
            var service = new StudySessionService(context, _mapper);

            // Extend the date range to ensure enough time
            var scheduleData = StudySessionDTOFactory.CreateSchedule(
                 studyPlanId: 1,
                 userId: 1,
                 startDate: DateTime.UtcNow.Date,
                 endDate: DateTime.UtcNow.Date.AddDays(6), // Ensure enough range
                 sessionsPerDay: 2,
                 sessionLength: 2, // Adjust session length
                 studyStartTime: DateTime.UtcNow.Date.AddHours(8),
                 studyEndTime: DateTime.UtcNow.Date.AddHours(10), // Extend study window
                 preferredDays: new List<string> { "Monday", "Tuesday", "Wednesday", "Thursday", "Saturday", "Sunday" },
                 topicIds: new List<int> { 1, 2 }
             );

            // Act
            var result = await service.GenerateAndStoreSchedule(scheduleData);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(5); // Adjust count based on input
        }

        [Fact]
        public async Task GenerateAndStoreSchedule_ShouldReturnNull_WhenNoPreferredStudyDays()
        {
            // Arrange
            var context = InMemoryDbContextFactory.Create("TestDb_NoPreferredStudyDays");
            await DatabaseSeeder.SeedStudySessions(context);
            var service = new StudySessionService(context, _mapper);

            var scheduleData = StudySessionDTOFactory.CreateSchedule(
                studyPlanId: 1,
                userId: 1,
                startDate: DateTime.UtcNow.Date,
                endDate: DateTime.UtcNow.Date.AddDays(2),
                sessionsPerDay: 3,
                sessionLength: 2,
                studyStartTime: DateTime.UtcNow.Date.AddHours(8),
                studyEndTime: DateTime.UtcNow.Date.AddHours(18),
                preferredDays: new List<string>(), // No days preferred
                topicIds: new List<int> { 1, 2 }
            );

            // Act
            var result = await service.GenerateAndStoreSchedule(scheduleData);

            // Assert
            result.Should().BeNull();
        }

        //This test validates that the script skips time slots that conflict with existing sessions
        //Specifically, it ensures that sessions are not created on UTC now or UTC now + 1 day at times (8 AM, 9 AM) where conflicts are present
        [Fact]
        public async Task GenerateAndStoreSchedule_ShouldSkipConflictingSessions()
        {
            // Arrange
            var context = InMemoryDbContextFactory.Create("TestDb_SkipConflictingSessions");
            await DatabaseSeeder.SeedConflictingStudySessions(context);
            var service = new StudySessionService(context, _mapper);

            var scheduleData = StudySessionDTOFactory.CreateSchedule(
                studyPlanId: 1,
                userId: 1,
                startDate: DateTime.UtcNow.Date,
                endDate: DateTime.UtcNow.Date.AddDays(15),
                sessionsPerDay: 3,
                sessionLength: 1,
                studyStartTime: DateTime.UtcNow.Date.AddHours(8),
                studyEndTime: DateTime.UtcNow.Date.AddHours(12),
                preferredDays: new List<string> { "Monday", "Tuesday", "Wednesday", "Thursday", "Saturday", "Sunday" },
                topicIds: new List<int> { 1, 2 }
            );

            // Act
            var result = await service.GenerateAndStoreSchedule(scheduleData);

            // Assert
            result.Should().NotBeNull();
            result!.Any(session => session.Date == DateTime.UtcNow.Date && session.StartTime == TimeSpan.FromHours(8)).Should().BeFalse();
            result!.Any(session => session.Date == DateTime.UtcNow.Date && session.StartTime == TimeSpan.FromHours(9)).Should().BeFalse();
            result!.Any(session => session.Date == DateTime.UtcNow.Date.AddDays(1) && session.StartTime == TimeSpan.FromHours(9)).Should().BeFalse();
            result!.Any(session => session.Date == DateTime.UtcNow.Date.AddDays(1) && session.StartTime == TimeSpan.FromHours(10)).Should().BeFalse();
        }

        [Fact]
        public async Task GenerateAndStoreSchedule_ShouldIgnoreNonexistentTopics()
        {
            // Arrange
            var context = InMemoryDbContextFactory.Create("TestDb_IgnoreNonexistentTopics");
            await DatabaseSeeder.SeedStudySessions(context);
            var service = new StudySessionService(context, _mapper);

            var scheduleData = StudySessionDTOFactory.CreateSchedule(
                studyPlanId: 1,
                userId: 1,
                startDate: DateTime.UtcNow.Date,
                endDate: DateTime.UtcNow.Date.AddDays(2),
                sessionsPerDay: 3,
                sessionLength: 2,
                studyStartTime: DateTime.UtcNow.Date.AddHours(8),
                studyEndTime: DateTime.UtcNow.Date.AddHours(18),
                preferredDays: new List<string> { "Monday", "Tuesday" },
                topicIds: new List<int> { 99, 100 }
            );

            // Act
            var result = await service.GenerateAndStoreSchedule(scheduleData);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GenerateAndStoreSchedule_ShouldRespectSessionsPerDayLimit()
        {
            // Arrange
            var context = InMemoryDbContextFactory.Create("TestDb_RespectSessionsPerDay");
            await DatabaseSeeder.SeedStudySessions(context);
            var service = new StudySessionService(context, _mapper);

            var scheduleData = StudySessionDTOFactory.CreateSchedule(
                studyPlanId: 1,
                userId: 1,
                startDate: DateTime.UtcNow.Date,
                endDate: DateTime.UtcNow.Date.AddDays(4), // Ensure enough days
                sessionsPerDay: 1,
                sessionLength: 1, // Ensure session fits in the study window
                studyStartTime: DateTime.UtcNow.Date.AddHours(8),
                studyEndTime: DateTime.UtcNow.Date.AddHours(18),
                preferredDays: new List<string> { "Monday", "Tuesday", "Wednesday", "Thursday", "Saturday", "Sunday" },
                topicIds: new List<int> { 1 }
            );

            // Act
            var result = await service.GenerateAndStoreSchedule(scheduleData);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(4); // One session per day for four preferred days
        }

        [Fact]
        public async Task GenerateAndStoreSchedule_ShouldReturnNull_WhenScheduleOutOfStudyPlanDates()
        {
            // Arrange
            var context = InMemoryDbContextFactory.Create("TestDb_StudyPlanDates");
            await DatabaseSeeder.SeedStudyPlans(context);
            var service = new StudySessionService(context, _mapper);

            var scheduleData = StudySessionDTOFactory.CreateSchedule(
                studyPlanId: 1,
                userId: 1,
                startDate: new DateTime(2024, 11, 28),
                endDate: new DateTime(2025, 11, 30), // Exceeds the StudyPlan's end date because the plan needs 4 days for all the sessions
                sessionsPerDay: 1,
                sessionLength: 1,
                studyStartTime: DateTime.UtcNow.Date.AddHours(9),
                studyEndTime: DateTime.UtcNow.Date.AddHours(12),
                preferredDays: new List<string> { "Monday", "Tuesday", "Wednesday", "Thursday", "Saturday", "Sunday" },
                topicIds: new List<int> { 1, 2 }
            );

            // Act
            var result = await service.GenerateAndStoreSchedule(scheduleData);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GenerateAndStoreSchedule_ShouldReturnNull_WhenTopicListIsEmpty()
        {
            // Arrange
            var context = InMemoryDbContextFactory.Create("TestDb_EmptyTopicList");
            await DatabaseSeeder.SeedStudyTopics(context);
            var service = new StudySessionService(context, _mapper);

            var scheduleData = StudySessionDTOFactory.CreateSchedule(
                studyPlanId: 1,
                userId: 1,
                startDate: DateTime.UtcNow.Date,
                endDate: DateTime.UtcNow.Date.AddDays(2),
                sessionsPerDay: 3,
                sessionLength: 2,
                studyStartTime: DateTime.UtcNow.Date.AddHours(8),
                studyEndTime: DateTime.UtcNow.Date.AddHours(18),
                preferredDays: new List<string> { "Monday", "Tuesday" },
                topicIds: new List<int>()
            );

            // Act
            var result = await service.GenerateAndStoreSchedule(scheduleData);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GenerateAndStoreSchedule_ShouldReturnNull_WhenStudyWindowIsInvalid()
        {
            // Arrange
            var context = InMemoryDbContextFactory.Create("TestDb_InvalidStudyWindow");
            await DatabaseSeeder.SeedStudyTopics(context);
            var service = new StudySessionService(context, _mapper);

            var scheduleData = StudySessionDTOFactory.CreateSchedule(
                studyPlanId: 1,
                userId: 1,
                startDate: DateTime.UtcNow.Date,
                endDate: DateTime.UtcNow.Date.AddDays(2),
                sessionsPerDay: 3,
                sessionLength: 2,
                studyStartTime: DateTime.UtcNow.Date.AddHours(13), // Start after end
                studyEndTime: DateTime.UtcNow.Date.AddHours(8),
                preferredDays: new List<string> { "Monday", "Tuesday" },
                topicIds: new List<int> { 1, 2 }
            );

            // Act
            var result = await service.GenerateAndStoreSchedule(scheduleData);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GenerateAndStoreSchedule_ShouldReturnNull_WhenNoPreferredDaysOverlap()
        {
            // Arrange
            var context = InMemoryDbContextFactory.Create("TestDb_NoPreferredDaysOverlap");
            await DatabaseSeeder.SeedStudyTopics(context);
            var service = new StudySessionService(context, _mapper);

            var fixedStartDate = new DateTime(2024, 12, 29);
            var scheduleData = StudySessionDTOFactory.CreateSchedule(
                studyPlanId: 1,
                userId: 1,
                startDate: fixedStartDate,
                endDate: fixedStartDate.AddDays(2),
                sessionsPerDay: 3,
                sessionLength: 2,
                studyStartTime: fixedStartDate.AddHours(8),
                studyEndTime: fixedStartDate.AddHours(18),
                preferredDays: new List<string> { "Friday", "Saturday" }, // No overlap with Monday-Wednesday
                topicIds: new List<int> { 1, 2 }
            );

            // Act
            var result = await service.GenerateAndStoreSchedule(scheduleData);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GenerateAndStoreSchedule_ShouldReturnNull_WhenTopicsExceedAvailableTime()
        {
            // Arrange
            var context = InMemoryDbContextFactory.Create("TestDb_ExceedAvailableTime");
            await DatabaseSeeder.SeedStudyTopics(context);
            var service = new StudySessionService(context, _mapper);

            var scheduleData = StudySessionDTOFactory.CreateSchedule(
                studyPlanId: 1,
                userId: 1,
                startDate: DateTime.UtcNow.Date,
                endDate: DateTime.UtcNow.Date.AddDays(1), // Limited time
                sessionsPerDay: 1,
                sessionLength: 1,
                studyStartTime: DateTime.UtcNow.Date.AddHours(8),
                studyEndTime: DateTime.UtcNow.Date.AddHours(9),
                preferredDays: new List<string> { "Monday", "Tuesday", "Friday" },
                topicIds: new List<int> { 1, 2 } // Requires more time than available
            );

            // Act
            var result = await service.GenerateAndStoreSchedule(scheduleData);

            // Assert
            result.Should().BeNull();
        }
    }
}