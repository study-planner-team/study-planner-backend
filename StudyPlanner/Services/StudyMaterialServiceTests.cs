using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using StudyPlannerAPI.Models.StudyMaterials;
using StudyPlannerAPI.Services.StudyMaterialServices;
using StudyPlannerTests.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudyPlannerTests.Services
{
    public class StudyMaterialServiceTests
    {
        private readonly IMapper _mapper;
        public StudyMaterialServiceTests()
        {
            _mapper = MapperFactory.GetMapper();
        }

        [Fact]
        public async Task GetMaterialsByTopicId_ShouldReturnMaterials_WhenTopicHasMaterials()
        {
            // Arrange
            var context = InMemoryDbContextFactory.Create("TestDb_GetMaterialsByTopicId");
            await DatabaseSeeder.SeedStudyMaterials(context);
            var service = new StudyMaterialService(context, _mapper);

            // Act
            var result = await service.GetMaterialsByTopicId(1);

            // Assert
            result.Should().NotBeEmpty();
            result.Should().AllBeOfType<StudyMaterialResponseDTO>();

            var material = result.FirstOrDefault(m => m.Title == "Material 1");
            material.Should().NotBeNull();
            material.Link.Should().Be("http://example.com/material1");
        }

        [Fact]
        public async Task GetMaterialsByTopicId_ShouldReturnEmpty_WhenTopicHasNoMaterials()
        {
            // Arrange
            var context = InMemoryDbContextFactory.Create("TestDb_GetMaterialsByTopicId_NoMaterials");
            await DatabaseSeeder.SeedStudyTopics(context);
            var service = new StudyMaterialService(context, _mapper);

            // Act
            var result = await service.GetMaterialsByTopicId(2);

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task CreateStudyMaterial_ShouldAddMaterial_WhenValidDataProvided()
        {
            // Arrange
            var context = InMemoryDbContextFactory.Create("TestDb_CreateStudyMaterial");
            await DatabaseSeeder.SeedStudyTopics(context);
            var service = new StudyMaterialService(context, _mapper);

            var materialDTO = new StudyMaterialDTO
            {
                Title = "New Material",
                Link = "http://example.com/new-material"
            };

            // Act
            var result = await service.CreateStudyMaterial(2, materialDTO);

            // Assert
            result.Should().NotBeNull();
            result.Title.Should().Be("New Material");
            result.Link.Should().Be("http://example.com/new-material");
            result.Should().BeOfType<StudyMaterialResponseDTO>();
        }

        [Fact]
        public async Task UpdateStudyMaterial_ShouldUpdateMaterial_WhenMaterialExists()
        {
            // Arrange
            var context = InMemoryDbContextFactory.Create("TestDb_UpdateStudyMaterial");
            await DatabaseSeeder.SeedStudyMaterials(context);
            var service = new StudyMaterialService(context, _mapper);

            var materialDTO = new StudyMaterialDTO
            {
                Title = "Updated Material",
                Link = "http://example.com/updated-material"
            };

            // Act
            var result = await service.UpdateStudyMaterial(1, materialDTO);

            // Assert
            result.Should().NotBeNull();
            result.Title.Should().Be("Updated Material");
            result.Link.Should().Be("http://example.com/updated-material");
            result.Should().BeOfType<StudyMaterialResponseDTO>();
        }

        [Fact]
        public async Task UpdateStudyMaterial_ShouldReturnNull_WhenMaterialDoesNotExist()
        {
            // Arrange
            var context = InMemoryDbContextFactory.Create("TestDb_UpdateNonexistentMaterial");
            await DatabaseSeeder.SeedStudyMaterials(context);
            var service = new StudyMaterialService(context, _mapper);

            var materialDTO = new StudyMaterialDTO
            {
                Title = "Updated Material",
                Link = "http://example.com/updated-material"
            };

            // Act
            var result = await service.UpdateStudyMaterial(99, materialDTO);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task DeleteStudyMaterial_ShouldRemoveMaterial_WhenMaterialExists()
        {
            // Arrange
            var context = InMemoryDbContextFactory.Create("TestDb_DeleteStudyMaterial");
            await DatabaseSeeder.SeedStudyMaterials(context);
            var service = new StudyMaterialService(context, _mapper);

            // Act
            var result = await service.DeleteStudyMaterial(1);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task DeleteStudyMaterial_ShouldReturnFalse_WhenMaterialDoesNotExist()
        {
            // Arrange
            var context = InMemoryDbContextFactory.Create("TestDb_DeleteNonexistentMaterial");
            await DatabaseSeeder.SeedStudyMaterials(context);
            var service = new StudyMaterialService(context, _mapper);

            // Act
            var result = await service.DeleteStudyMaterial(99);

            // Assert
            result.Should().BeFalse();
        }
    }
}
