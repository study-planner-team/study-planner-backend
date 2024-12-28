using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using StudyPlannerAPI.Models.StudyMaterials;
using StudyPlannerAPI.Models.StudyTopics;
using StudyPlannerAPI.Services.StudyTopicServices;
using StudyPlannerTests.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudyPlannerTests.Services
{
    public class StudyTopicServiceTest
    {
        private readonly IMapper _mapper;
        public StudyTopicServiceTest() 
        {
            _mapper = MapperFactory.GetMapper();
        }
        [Fact]
        public async Task GetTopicsForStudyPlan_ShouldReturnTopicsWithMaterials_WhenStudyPlanExists()
        {
            // Arrange
            var context = InMemoryDbContextFactory.Create("TestDb_GetTopicsForStudyPlan");
            await DatabaseSeeder.SeedStudyTopics(context);
            var service = new StudyTopicService(context, _mapper);

            // Act
            var result = await service.GetTopicsForStudyPlan(1);

            // Assert
            result.Should().NotBeEmpty();
            result.Should().HaveCount(2);
            result.Should().AllBeOfType<StudyTopicResponseDTO>();

            var topic = result.FirstOrDefault(t => t.Title == "Study Topic 1");
            topic.Should().NotBeNull();
            topic.Hours.Should().Be(8);
            topic.StudyMaterials.Should().NotBeEmpty();
            topic.StudyMaterials.Should().AllBeOfType<StudyMaterialResponseDTO>();
        }

        [Fact]
        public async Task GetTopicsForStudyPlan_ShouldReturnEmptyList_WhenStudyPlanHasNoTopics()
        {
            // Arrange
            var context = InMemoryDbContextFactory.Create("TestDb_GetTopicsForStudyPlan_NoTopics");
            await DatabaseSeeder.SeedStudyPlans(context);
            var service = new StudyTopicService(context, _mapper);

            // Act
            var result = await service.GetTopicsForStudyPlan(1);

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task AddTopicToStudyPlan_ShouldAddTopic_WhenValidDataProvided()
        {
            // Arrange
            var context = InMemoryDbContextFactory.Create("TestDb_AddTopicToStudyPlan");
            await DatabaseSeeder.SeedStudyPlans(context);
            var service = new StudyTopicService(context, _mapper);

            var topicDTO = new StudyTopicDTO
            {
                Title = "New Topic",
                Hours = 25
            };

            // Act
            var result = await service.AddTopicToStudyPlan(1, topicDTO);

            // Assert
            result.Should().NotBeNull();
            result.Title.Should().Be("New Topic");
            result.Hours.Should().Be(25);
            result.Should().BeOfType<StudyTopic>();
        }

        [Fact]
        public async Task DeleteStudyTopic_ShouldRemoveTopic_WhenTopicExists()
        {
            // Arrange
            var context = InMemoryDbContextFactory.Create("TestDb_DeleteStudyTopic");
            await DatabaseSeeder.SeedStudyTopics(context);
            var service = new StudyTopicService(context, _mapper);

            // Act
            var result = await service.DeleteStudyTopic(1);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task DeleteStudyTopic_ShouldReturnFalse_WhenTopicDoesNotExist()
        {
            // Arrange
            var context = InMemoryDbContextFactory.Create("TestDb_DeleteNonexistentStudyTopic");
            await DatabaseSeeder.SeedStudyTopics(context);
            var service = new StudyTopicService(context, _mapper);

            // Act
            var result = await service.DeleteStudyTopic(99);

            // Assert
            result.Should().BeFalse();
        }
    }
}
