using AutoMapper;
using FluentAssertions;
using StudyPlannerAPI.Models.StudyPlans;
using StudyPlannerAPI.Models.Users;
using StudyPlannerAPI.Services.StudyPlanServices;
using StudyPlannerTests.Common;

namespace StudyPlannerTests.Services
{
    public class StudyPlanServiceTests
    {
        private readonly IMapper _mapper;

        public StudyPlanServiceTests()
        {
            _mapper = MapperFactory.GetMapper();
        }

        [Fact]
        public async Task CreateStudyPlan_ShouldAddPlanToDatabase()
        {
            // Arrange
            var context = InMemoryDbContextFactory.Create("TestDb_CreateStudyPlan");
            await DatabaseSeeder.SeedStudyPlans(context);
            var service = new StudyPlanService(context, _mapper);

            var newPlanDto = new StudyPlanDTO
            {
                Title = "New Plan",
                Description = "New study plan",
                Category = "Category A",
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(10),
                IsPublic = false
            };

            // Act
            var result = await service.CreateStudyPlan(1, newPlanDto);

            // Assert
            result.Should().NotBeNull();
            result.Title.Should().Be("New Plan");
            result.Should().BeOfType<StudyPlanResponseDTO>();
            result.Owner.Should().NotBeNull();
            result.Owner.UserId.Should().Be(1);
        }

        [Fact]
        public async Task CreateStudyPlan_ShouldThrowArgumentNullException_WhenDtoIsNull()
        {
            // Arrange
            var context = InMemoryDbContextFactory.Create("TestDb_CreateStudyPlan_NullDto");
            var service = new StudyPlanService(context, _mapper);

            // Act & Assert
            await Assert.ThrowsAsync<NullReferenceException>(() => service.CreateStudyPlan(1, null));
        }

        [Fact]
        public async Task DeleteStudyPlan_ShouldRemovePlanFromDatabase()
        {
            // Arrange
            var context = InMemoryDbContextFactory.Create("TestDb_DeleteStudyPlan");
            await DatabaseSeeder.SeedStudyPlans(context);
            var service = new StudyPlanService(context, _mapper);

            // Act
            var result = await service.DeleteStudyPlan(1);

            // Assert
            result.Should().BeTrue();

            var dbPlan = await context.StudyPlans.FindAsync(1);
            dbPlan.Should().BeNull();
        }

        [Fact]
        public async Task DeleteStudyPlan_ShouldReturnFalse_WhenPlanDoesNotExist()
        {
            // Arrange
            var context = InMemoryDbContextFactory.Create("TestDb_DeleteNonexistentPlan");
            var service = new StudyPlanService(context, _mapper);

            // Act
            var result = await service.DeleteStudyPlan(99);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task GetStudyPlanById_ShouldReturnCorrectPlan()
        {
            // Arrange
            var context = InMemoryDbContextFactory.Create("TestDb_GetStudyPlanById");
            await DatabaseSeeder.SeedStudyPlans(context);
            var service = new StudyPlanService(context, _mapper);

            // Act
            var result = await service.GetStudyPlanById(1);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<StudyPlanResponseDTO>();
            result.Title.Should().Be("Plan Owned by User 1");
            result.Owner.Should().NotBeNull();
            result.Owner.UserId.Should().Be(1);
        }

        [Fact]
        public async Task GetStudyPlanById_ShouldReturnNull_WhenPlanDoesNotExist()
        {
            // Arrange
            var context = InMemoryDbContextFactory.Create("TestDb_GetNonexistentStudyPlanById");
            await DatabaseSeeder.SeedStudyPlans(context);
            var service = new StudyPlanService(context, _mapper);

            // Act
            var result = await service.GetStudyPlanById(99);

            // Assert
            result.Should().BeNull(); 
        }

        [Fact]
        public async Task GetStudyPlansForUser_ShouldReturnActivePlans()
        {
            // Arrange
            var context = InMemoryDbContextFactory.Create("TestDb_GetStudyPlansForUser");
            await DatabaseSeeder.SeedStudyPlans(context);
            var service = new StudyPlanService(context, _mapper);

            // Act
            var result = await service.GetStudyPlansForUser(1);

            // Assert
            result.Should().NotBeEmpty();
            result.Should().AllBeOfType<StudyPlanResponseDTO>();
            result.Should().OnlyContain(plan => plan.IsArchived == false);
        }

        [Fact]
        public async Task GetStudyPlansForUser_ShouldReturnEmpty_WhenUserHasNoActivePlans()
        {
            // Arrange
            var context = InMemoryDbContextFactory.Create("TestDb_UserWithNoActivePlans");
            await DatabaseSeeder.SeedStudyPlans(context);
            var service = new StudyPlanService(context, _mapper);

            // Act
            var result = await service.GetStudyPlansForUser(2); 

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task UpdateStudyPlan_ShouldUpdatePlanInDatabase()
        {
            // Arrange
            var context = InMemoryDbContextFactory.Create("TestDb_UpdateStudyPlan");
            await DatabaseSeeder.SeedStudyPlans(context);
            var service = new StudyPlanService(context, _mapper);

            var updateDto = new StudyPlanDTO
            {
                Title = "Updated Title",
                Description = "Updated Description",
                Category = "Updated Category",
                StartDate = DateTime.UtcNow.AddDays(-5),
                EndDate = DateTime.UtcNow.AddDays(5),
                IsPublic = true
            };

            // Act
            var result = await service.UpdateStudyPlan(1, updateDto);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<StudyPlanResponseDTO>();
            result.Title.Should().Be("Updated Title");
            result.Description.Should().Be("Updated Description");
            result.Category.Should().Be("Updated Category");
            result.IsPublic.Should().BeTrue();
        }

        [Fact]
        public async Task UpdateStudyPlan_ShouldReturnNull_WhenPlanDoesNotExist()
        {
            // Arrange
            var context = InMemoryDbContextFactory.Create("TestDb_UpdateNonexistentPlan");
            var service = new StudyPlanService(context, _mapper);

            var updateDto = new StudyPlanDTO
            {
                Title = "Nonexistent Title",
                Description = "Nonexistent Description",
                Category = "Nonexistent Category",
                StartDate = DateTime.UtcNow.AddDays(-5),
                EndDate = DateTime.UtcNow.AddDays(5),
                IsPublic = true
            };

            // Act
            var result = await service.UpdateStudyPlan(99, updateDto);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task ArchiveStudyPlan_ShouldSetIsArchivedToTrue()
        {
            // Arrange
            var context = InMemoryDbContextFactory.Create("TestDb_ArchiveStudyPlan");
            await DatabaseSeeder.SeedStudyPlans(context);
            var service = new StudyPlanService(context, _mapper);

            // Act
            var result = await service.ArchiveStudyPlan(1);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<StudyPlanResponseDTO>();
            result.IsArchived.Should().BeTrue();
            result.Title.Should().Be("Plan Owned by User 1");
        }

        [Fact]
        public async Task ArchiveStudyPlan_ShouldReturnNull_WhenPlanDoesNotExist()
        {
            // Arrange
            var context = InMemoryDbContextFactory.Create("TestDb_ArchiveNonexistentPlan");
            var service = new StudyPlanService(context, _mapper);

            // Act
            var result = await service.ArchiveStudyPlan(99);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task UnarchiveStudyPlan_ShouldSetIsArchivedToFalse()
        {
            // Arrange
            var context = InMemoryDbContextFactory.Create("TestDb_UnarchiveStudyPlan");
            await DatabaseSeeder.SeedStudyPlans(context);
            var service = new StudyPlanService(context, _mapper);

            // Act
            var result = await service.UnarchiveStudyPlan(2);

            // Assert
            result.Should().NotBeNull();
            result.IsArchived.Should().BeFalse();
        }

        [Fact]
        public async Task UnarchiveStudyPlan_ShouldReturnNull_WhenPlanDoesNotExist()
        {
            // Arrange
            var context = InMemoryDbContextFactory.Create("TestDb_UnarchiveNonExistentPlan");
            var service = new StudyPlanService(context, _mapper);

            // Act
            var result = await service.UnarchiveStudyPlan(99); 

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetArchivedStudyPlansForUser_ShouldReturnArchivedPlans()
        {
            // Arrange
            var context = InMemoryDbContextFactory.Create("TestDb_GetArchivedStudyPlansForUser");
            await DatabaseSeeder.SeedStudyPlans(context);
            var service = new StudyPlanService(context, _mapper);

            // Act
            var result = await service.GetArchivedStudyPlansForUser(2);

            // Assert
            result.Should().NotBeEmpty();
            result.Should().AllBeOfType<StudyPlanResponseDTO>();
            result.Should().OnlyContain(plan => plan.IsArchived == true);
        }

        [Fact]
        public async Task GetArchivedStudyPlansForUser_ShouldReturnEmpty_WhenUserHasNoArchivedPlans()
        {
            // Arrange
            var context = InMemoryDbContextFactory.Create("TestDb_UserWithNoArchivedPlans");
            await DatabaseSeeder.SeedStudyPlans(context);
            var service = new StudyPlanService(context, _mapper);

            // Act
            var result = await service.GetArchivedStudyPlansForUser(1);

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetPublicStudyPlans_ShouldReturnPublicPlansExcludingOwnedAndMemberPlans()
        {
            // Arrange
            var context = InMemoryDbContextFactory.Create("TestDb_GetPublicStudyPlans");
            await DatabaseSeeder.SeedStudyPlans(context);
            var service = new StudyPlanService(context, _mapper);

            // Act
            var result = await service.GetPublicStudyPlans(3);

            // Assert
            result.Should().NotBeEmpty();
            result.Should().AllBeOfType<StudyPlanResponseDTO>();
            result.Should().OnlyContain(plan => plan.IsPublic == true && plan.Owner.UserId != 2);
        }

        [Fact]
        public async Task GetPublicStudyPlans_ShouldReturnEmpty_WhenNoPublicPlansExist()
        {
            // Arrange
            var context = InMemoryDbContextFactory.Create("TestDb_NoPublicStudyPlans");
            await DatabaseSeeder.SeedStudyPlans(context);
            var service = new StudyPlanService(context, _mapper);

            // Act
            var result = await service.GetPublicStudyPlans(1); 

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetJoinedStudyPlansForUser_ShouldReturnJoinedPlans()
        {
            // Arrange
            var context = InMemoryDbContextFactory.Create("TestDb_GetJoinedStudyPlans");
            await DatabaseSeeder.SeedStudyPlans(context);
            var service = new StudyPlanService(context, _mapper);

            // Act
            var result = await service.GetJoinedStudyPlansForUser(2); 

            // Assert
            result.Should().NotBeEmpty();
            result.Should().AllBeOfType<StudyPlanResponseDTO>();
            result.Should().OnlyContain(plan => plan.Owner.UserId != 2);
            result.Should().Contain(plan => plan.Title == "Plan Owned by User 1");
        }

        [Fact]
        public async Task GetJoinedStudyPlansForUser_ShouldReturnEmpty_WhenUserHasNoJoinedPlans()
        {
            // Arrange
            var context = InMemoryDbContextFactory.Create("TestDb_UserWithNoJoinedPlans");
            await DatabaseSeeder.SeedStudyPlans(context);
            var service = new StudyPlanService(context, _mapper);

            // Act
            var result = await service.GetJoinedStudyPlansForUser(3);

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task JoinPublicStudyPlan_ShouldAddUserAsMember_WhenPlanIsPublic()
        {
            // Arrange
            var context = InMemoryDbContextFactory.Create("TestDb_JoinPublicStudyPlan");
            await DatabaseSeeder.SeedStudyPlans(context);
            var service = new StudyPlanService(context, _mapper);

            // Act
            var result = await service.JoinPublicStudyPlan(3, 1);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task JoinPublicStudyPlan_ShouldReturnFalse_WhenPlanIsNotPublic()
        {
            // Arrange
            var context = InMemoryDbContextFactory.Create("TestDb_JoinNonPublicStudyPlan");
            await DatabaseSeeder.SeedStudyPlans(context);
            var service = new StudyPlanService(context, _mapper);

            // Act
            var result = await service.JoinPublicStudyPlan(3, 2);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task JoinPublicStudyPlan_ShouldReturnFalse_WhenPlanDoesNotExist()
        {
            // Arrange
            var context = InMemoryDbContextFactory.Create("TestDb_JoinNonExistentPlan");
            await DatabaseSeeder.SeedStudyPlans(context);
            var service = new StudyPlanService(context, _mapper);

            // Act
            var result = await service.JoinPublicStudyPlan(3, 99);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task JoinPublicStudyPlan_ShouldReturnFalse_WhenUserIsAlreadyMember()
        {
            // Arrange
            var context = InMemoryDbContextFactory.Create("TestDb_JoinExistingMembership");
            await DatabaseSeeder.SeedStudyPlans(context);
            var service = new StudyPlanService(context, _mapper);

            // Act
            var result = await service.JoinPublicStudyPlan(1, 2);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task GetStudyPlanMembers_ShouldReturnMembers_WhenPlanHasMembers()
        {
            // Arrange
            var context = InMemoryDbContextFactory.Create("TestDb_GetStudyPlanMembers_WithMembers");
            await DatabaseSeeder.SeedStudyPlans(context);
            var service = new StudyPlanService(context, _mapper);

            // Act
            var result = await service.GetStudyPlanMembers(1);

            // Assert
            result.Should().NotBeEmpty();
            result.Should().AllBeOfType<UserResponseDTO>();
            result.Should().Contain(member => member.Username == "MemberUser");
        }

        [Fact]
        public async Task GetStudyPlanMembers_ShouldReturnEmpty_WhenPlanHasNoMembers()
        {
            // Arrange
            var context = InMemoryDbContextFactory.Create("TestDb_GetStudyPlanMembers_NoMembers");
            await DatabaseSeeder.SeedStudyPlans(context);
            var service = new StudyPlanService(context, _mapper);

            // Act
            var result = await service.GetStudyPlanMembers(3);

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetStudyPlanMembers_ShouldReturnEmpty_WhenPlanDoesNotExist()
        {
            // Arrange
            var context = InMemoryDbContextFactory.Create("TestDb_GetStudyPlanMembers_PlanDoesNotExist");
            await DatabaseSeeder.SeedStudyPlans(context);
            var service = new StudyPlanService(context, _mapper);

            // Act
            var result = await service.GetStudyPlanMembers(99);

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task LeavePublicStudyPlan_ShouldRemoveMembership_WhenUserIsMember()
        {
            // Arrange
            var context = InMemoryDbContextFactory.Create("TestDb_LeavePublicStudyPlan");
            await DatabaseSeeder.SeedStudyPlans(context);
            var service = new StudyPlanService(context, _mapper);

            // Act
            var result = await service.LeavePublicStudyPlan(2, 1);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task LeavePublicStudyPlan_ShouldReturnFalse_WhenUserIsNotMember()
        {
            // Arrange
            var context = InMemoryDbContextFactory.Create("TestDb_LeaveNonMemberPlan");
            await DatabaseSeeder.SeedStudyPlans(context);
            var service = new StudyPlanService(context, _mapper);

            // Act
            var result = await service.LeavePublicStudyPlan(3, 1);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task LeavePublicStudyPlan_ShouldReturnFalse_WhenPlanDoesNotExist()
        {
            // Arrange
            var context = InMemoryDbContextFactory.Create("TestDb_LeaveNonExistentPlan");
            await DatabaseSeeder.SeedStudyPlans(context);
            var service = new StudyPlanService(context, _mapper);

            // Act
            var result = await service.LeavePublicStudyPlan(2, 99);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task ChangeStudyPlanOwner_ShouldUpdateOwner_WhenConditionsAreMet()
        {
            // Arrange
            var context = InMemoryDbContextFactory.Create("TestDb_ChangeOwner");
            await DatabaseSeeder.SeedStudyPlans(context);
            var service = new StudyPlanService(context, _mapper);

            // Act
            var result = await service.ChangeStudyPlanOwner(1, 1, 2); // User 1 transfers ownership of Plan 1 to User 2

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task ChangeStudyPlanOwner_ShouldReturnFalse_WhenPlanDoesNotExist()
        {
            // Arrange
            var context = InMemoryDbContextFactory.Create("TestDb_ChangeOwner_NonExistentPlan");
            await DatabaseSeeder.SeedStudyPlans(context);
            var service = new StudyPlanService(context, _mapper);

            // Act
            var result = await service.ChangeStudyPlanOwner(1, 99, 2);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task ChangeStudyPlanOwner_ShouldReturnFalse_WhenCurrentUserIsNotOwner()
        {
            // Arrange
            var context = InMemoryDbContextFactory.Create("TestDb_ChangeOwner_NotOwner");
            await DatabaseSeeder.SeedStudyPlans(context);
            var service = new StudyPlanService(context, _mapper);

            // Act
            var result = await service.ChangeStudyPlanOwner(2, 1, 3); // User 2 is not the owner of Plan 1

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task ChangeStudyPlanOwner_ShouldReturnFalse_WhenNewOwnerIsNotMember()
        {
            // Arrange
            var context = InMemoryDbContextFactory.Create("TestDb_ChangeOwner_NotMember");
            await DatabaseSeeder.SeedStudyPlans(context);
            var service = new StudyPlanService(context, _mapper);

            // Act
            var result = await service.ChangeStudyPlanOwner(1, 1, 3); // User 3 is not a member of Plan 1

            // Assert
            result.Should().BeFalse();
        }
    }
}
