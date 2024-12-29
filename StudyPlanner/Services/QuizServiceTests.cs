using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using StudyPlannerAPI.Models.Quizes;
using StudyPlannerAPI.Models.Quizes.RequestDTOs;
using StudyPlannerAPI.Models.Quizes.ResponseDTOs;
using StudyPlannerAPI.Services.QuizService;
using StudyPlannerTests.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudyPlannerTests.Services
{
    public class QuizServiceTests
    {
        private readonly IMapper _mapper;

        public QuizServiceTests()
        {
            _mapper = MapperFactory.GetMapper();
        }

        [Fact]
        public async Task CreateQuizWithQuestions_ShouldCreateQuizAndReturnQuizResponse()
        {
            // Arrange
            var context = InMemoryDbContextFactory.Create("TestDb_CreateQuizWithQuestions");
            var service = new QuizService(context, _mapper);

            var quizDto = new QuizRequestDTO
            {
                Title = "New Quiz",
                Description = "This is a test quiz.",
                Questions = new List<QuestionRequestDTO>
                {
                    new QuestionRequestDTO
                    {
                        QuestionText = "What is AI?",
                        Options = new List<QuestionOptionRequestDTO>
                        {
                            new QuestionOptionRequestDTO { OptionText = "Artificial Intelligence", IsCorrect = true },
                            new QuestionOptionRequestDTO { OptionText = "Machine Learning", IsCorrect = false }
                        }
                    }
                }
            };

            // Act
            var result = await service.CreateQuizWithQuestions(1, quizDto, 1);

            // Assert
            result.Should().NotBeNull();
            result.QuizId.Should().BeGreaterThan(0);
            result.Questions.Should().HaveCount(1);
            result.Questions.First().Options.Should().HaveCount(2);
            result.Should().BeOfType<QuizResponseDTO>();
            result.Questions.Should().AllBeOfType<QuestionResponseDTO>();
            result.Questions.First().Options.Should().AllBeOfType<QuestionOptionResponseDTO>();
        }

        [Fact]
        public async Task GetQuizById_ShouldReturnQuiz_WhenQuizExists()
        {
            // Arrange
            var context = InMemoryDbContextFactory.Create("TestDb_GetQuizById");
            await DatabaseSeeder.SeedQuizzes(context);
            var service = new QuizService(context, _mapper);

            // Act
            var result = await service.GetQuizById(1);

            // Assert
            result.Should().NotBeNull();
            result!.QuizId.Should().Be(1);
            result.Questions.Should().HaveCount(2);
            result.Questions.First().Options.Should().HaveCount(2);
            result.Should().BeOfType<QuizResponseDTO>();
            result.Questions.Should().AllBeOfType<QuestionResponseDTO>();
            result.Questions.First().Options.Should().AllBeOfType<QuestionOptionResponseDTO>();
        }

        [Fact]
        public async Task GetQuizById_ShouldReturnNull_WhenQuizDoesNotExist()
        {
            // Arrange
            var context = InMemoryDbContextFactory.Create("TestDb_GetQuizById_NonExistent");
            var service = new QuizService(context, _mapper);

            // Act
            var result = await service.GetQuizById(99);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task DeleteQuiz_ShouldRemoveQuiz_WhenQuizExists()
        {
            // Arrange
            var context = InMemoryDbContextFactory.Create("TestDb_DeleteQuiz");
            await DatabaseSeeder.SeedQuizzes(context);
            var service = new QuizService(context, _mapper);

            // Act
            var result = await service.DeleteQuiz(1);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task DeleteQuiz_ShouldReturnFalse_WhenQuizDoesNotExist()
        {
            // Arrange
            var context = InMemoryDbContextFactory.Create("TestDb_DeleteQuiz_NonExistent");
            var service = new QuizService(context, _mapper);

            // Act
            var result = await service.DeleteQuiz(99);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task AssignQuizToUser_ShouldAssignQuiz_WhenNotAlreadyAssigned()
        {
            // Arrange
            var context = InMemoryDbContextFactory.Create("TestDb_AssignQuizToUser");
            await DatabaseSeeder.SeedQuizzes(context);
            var service = new QuizService(context, _mapper);

            // Act
            var result = await service.AssignQuizToUser(1, 2);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task AssignQuizToUser_ShouldReturnFalse_WhenAlreadyAssigned()
        {
            // Arrange
            var context = InMemoryDbContextFactory.Create("TestDb_AssignQuizToUser_AlreadyAssigned");
            await DatabaseSeeder.SeedQuizAssignments(context);
            var service = new QuizService(context, _mapper);

            // Act
            var result = await service.AssignQuizToUser(1, 1);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task GetAssignedQuizzes_ShouldReturnAssignedQuizzes_WhenAssignmentsExist()
        {
            // Arrange
            var context = InMemoryDbContextFactory.Create("TestDb_GetAssignedQuizzes");
            await DatabaseSeeder.SeedQuizAssignments(context);
            var service = new QuizService(context, _mapper);

            // Act
            var result = await service.GetAssignedQuizzes(1);

            // Assert
            result.Should().NotBeEmpty();
            result.Should().HaveCount(2);
            result.Should().AllBeOfType<QuizAssignmentResponseDTO>();
        }

        [Fact]
        public async Task GetAssignedQuizzes_ShouldReturnEmptyList_WhenNoAssignmentsExist()
        {
            // Arrange
            var context = InMemoryDbContextFactory.Create("TestDb_GetAssignedQuizzes_NoAssignments");
            await DatabaseSeeder.SeedQuizAssignments(context);
            var service = new QuizService(context, _mapper);

            // Act
            var result = await service.GetAssignedQuizzes(99);

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetAssignedQuizById_ShouldReturnQuizAssignment_WhenAssignmentExists()
        {
            // Arrange
            var context = InMemoryDbContextFactory.Create("TestDb_GetAssignedQuizById");
            await DatabaseSeeder.SeedQuizAssignments(context);
            var service = new QuizService(context, _mapper);

            // Act
            var result = await service.GetAssignedQuizById(1, 1);

            // Assert
            result.Should().NotBeNull();
            result!.Quiz.QuizId.Should().Be(1);
            result.AssignmentId.Should().Be(1);
            result.Should().BeOfType<QuizAssignmentResponseDTO>();
        }

        [Fact]
        public async Task GetAssignedQuizById_ShouldReturnNull_WhenAssignmentDoesNotExist()
        {
            // Arrange
            var context = InMemoryDbContextFactory.Create("TestDb_GetAssignedQuizById_NonExistent");
            var service = new QuizService(context, _mapper);

            // Act
            var result = await service.GetAssignedQuizById(1, 99);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetCreatedQuizzes_ShouldReturnQuizzes_WhenUserHasCreatedQuizzes()
        {
            // Arrange
            var context = InMemoryDbContextFactory.Create("TestDb_GetCreatedQuizzes");
            await DatabaseSeeder.SeedQuizzes(context);
            var service = new QuizService(context, _mapper);

            // Act
            var result = await service.GetCreatedQuizzes(1);

            // Assert
            result.Should().NotBeEmpty();
            result.Should().HaveCount(2);
            result.First().Title.Should().Be("Quiz 1");
            result.Should().AllBeOfType<QuizResponseDTO>();
        }

        [Fact]
        public async Task GetCreatedQuizzes_ShouldReturnEmpty_WhenUserHasNoQuizzes()
        {
            // Arrange
            var context = InMemoryDbContextFactory.Create("TestDb_GetCreatedQuizzes_Empty");
            await DatabaseSeeder.SeedQuizzes(context);
            var service = new QuizService(context, _mapper);

            // Act
            var result = await service.GetCreatedQuizzes(99);

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task UpdateQuizScore_ShouldUpdateCorrectAnswersAndMarkAsCompleted()
        {
            // Arrange
            var context = InMemoryDbContextFactory.Create("TestDb_UpdateQuizScore");
            await DatabaseSeeder.SeedQuizAssignments(context);
            var service = new QuizService(context, _mapper);

            // Prepare user answers
            var userAnswers = new List<UserAnswerDTO>
            {
                new UserAnswerDTO { QuestionId = 11, SelectedOptionId = 111 }, // Correct answer
                new UserAnswerDTO { QuestionId = 12, SelectedOptionId = 122 }  // Wrong answer
            };

            // Act
            var result = await service.UpdateQuizScore(1, userAnswers);

            // Assert
            result.Should().NotBeNull();
            result!.CorrectAnswers.Should().Be(1);
            result.TotalQuestions.Should().Be(2);
            result.State.Should().Be("Completed");
        }

        [Fact]
        public async Task UpdateQuizScore_ShouldReturnNull_WhenAssignmentDoesNotExist()
        {
            // Arrange
            var context = InMemoryDbContextFactory.Create("TestDb_UpdateQuizScore_NonExistent");
            var service = new QuizService(context, _mapper);

            // Prepare user answers
            var userAnswers = new List<UserAnswerDTO>
            {
                new UserAnswerDTO { QuestionId = 11, SelectedOptionId = 111 },
                new UserAnswerDTO { QuestionId = 12, SelectedOptionId = 122 }
            };

            // Act
            var result = await service.UpdateQuizScore(99, userAnswers);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetCompletedQuizzes_ShouldReturnCompletedQuizzes_WhenUserHasCompletedQuizzes()
        {
            // Arrange
            var context = InMemoryDbContextFactory.Create("TestDb_GetCompletedQuizzes");
            await DatabaseSeeder.SeedQuizAssignments(context);
            var service = new QuizService(context, _mapper);

            // Act
            var result = await service.GetCompletedQuizzes(1);

            // Assert
            result.Should().NotBeEmpty();
            result.Should().HaveCount(1);
            result.First().State.Should().Be("Completed");
            result.Should().AllBeOfType<QuizAssignmentResponseDTO>();
        }

        [Fact]
        public async Task GetCompletedQuizzes_ShouldReturnEmpty_WhenUserHasNoCompletedQuizzes()
        {
            // Arrange
            var context = InMemoryDbContextFactory.Create("TestDb_GetCompletedQuizzes_Empty");
            await DatabaseSeeder.SeedQuizAssignments(context);
            var service = new QuizService(context, _mapper);

            // Act
            var result = await service.GetCompletedQuizzes(99);

            // Assert
            result.Should().BeEmpty();
        }
    }
}
