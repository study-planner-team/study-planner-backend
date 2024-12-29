using AutoMapper;
using FluentAssertions;
using StudyPlannerAPI.Models.StudySessions;
using StudyPlannerAPI.Services.StudySessionsServices;
using StudyPlannerTests.Common;
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
    }
}
