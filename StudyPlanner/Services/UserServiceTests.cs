using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using StudyPlannerAPI.Models.Users;
using StudyPlannerAPI.Services.UserServices;
using StudyPlannerTests.Common;

namespace StudyPlannerTests.Services
{
    public class UserServiceTests
    {
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public UserServiceTests()
        {
            _mapper = MapperFactory.GetMapper();
            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "Jwt:Key", "SecretKeyForTestingPurposes12345" },
                    { "Jwt:Issuer", "TestIssuer" },
                    { "Jwt:Audience", "TestAudience" }
                })
                .Build();
        }

        [Fact]
        public async Task RegisterUser_ShouldAddUserToDatabase()
        {
            // Arrange
            var context = InMemoryDbContextFactory.Create("TestDb_RegisterUser");
            var service = new UserService(context, _mapper, _configuration);

            var userDTO = new UserRegistrationDTO
            {
                Username = "NewUser",
                Email = "newuser@test.com",
                Password = "SecurePassword123"
            };

            // Act
            var result = await service.RegisterUser(userDTO);

            // Assert
            result.Should().NotBeNull();
            result.Username.Should().Be("NewUser");
            result.Email.Should().Be("newuser@test.com");
        }

        [Fact]
        public async Task LoginUser_ShouldReturnTokensAndUser_WhenCredentialsAreValid()
        {
            // Arrange
            var context = InMemoryDbContextFactory.Create("TestDb_LoginUser");
            await DatabaseSeeder.SeedUsers(context);
            var service = new UserService(context, _mapper, _configuration);

            var loginDTO = new UserLoginDTO
            {
                Username = "OwnerUser",
                Password = "Test123!"
            };

            // Act
            var result = await service.LoginUser(loginDTO);

            // Assert
            result.Item1.Should().NotBeNull();
            result.Item2.Should().NotBeNull();
            result.Item3.Should().NotBeNull();
            result.Item3.Should().BeOfType<UserResponseDTO>();
            result.Item3.Username.Should().Be("OwnerUser");
        }

        [Fact]
        public async Task LoginUser_ShouldReturnNull_WhenCredentialsAreInvalid()
        {
            // Arrange
            var context = InMemoryDbContextFactory.Create("TestDb_LoginUser_Invalid");
            await DatabaseSeeder.SeedUsers(context);
            var service = new UserService(context, _mapper, _configuration);

            var loginDTO = new UserLoginDTO
            {
                Username = "OwnerUser",
                Password = "wrongpassword"
            };

            // Act
            var result = await service.LoginUser(loginDTO);

            // Assert
            result.Item1.Should().BeNull();
            result.Item2.Should().BeNull();
            result.Item3.Should().BeNull();
        }

        [Fact]
        public async Task RefreshToken_ShouldReturnNewTokens_WhenRefreshTokenIsValid()
        {
            // Arrange
            var context = InMemoryDbContextFactory.Create("TestDb_RefreshToken");
            await DatabaseSeeder.SeedUsers(context);
            var service = new UserService(context, _mapper, _configuration);

            var validRefreshToken = "valid-refresh-token";

            // Act
            var result = await service.RefreshToken(validRefreshToken);

            // Assert
            result.Item1.Should().NotBeNull(); // Access token
            result.Item2.Should().NotBeNull(); // Refresh token
        }

        [Fact]
        public async Task RefreshToken_ShouldReturnNull_WhenRefreshTokenIsInvalid()
        {
            // Arrange
            var context = InMemoryDbContextFactory.Create("TestDb_RefreshToken_Invalid");
            await DatabaseSeeder.SeedUsers(context);
            var service = new UserService(context, _mapper, _configuration);

            var invalidRefreshToken = "invalid-refresh-token";

            // Act
            var result = await service.RefreshToken(invalidRefreshToken);

            // Assert
            result.Item1.Should().BeNull();
            result.Item2.Should().BeNull();
        }

        [Fact]
        public async Task LogoutUser_ShouldInvalidateRefreshToken_WhenRefreshTokenIsValid()
        {
            // Arrange
            var context = InMemoryDbContextFactory.Create("TestDb_LogoutUser");
            await DatabaseSeeder.SeedUsers(context);
            var service = new UserService(context, _mapper, _configuration);

            var validRefreshToken = "valid-refresh-token";

            // Act
            var result = await service.LogoutUser(validRefreshToken);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task LogoutUser_ShouldReturnFalse_WhenRefreshTokenIsNotFound()
        {
            // Arrange
            var context = InMemoryDbContextFactory.Create("TestDb_LogoutUser_Invalid");
            await DatabaseSeeder.SeedUsers(context);
            var service = new UserService(context, _mapper, _configuration);

            var invalidRefreshToken = "invalid-refresh-token";

            // Act
            var result = await service.LogoutUser(invalidRefreshToken);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task UpdateUser_ShouldUpdateUser_WhenUserExists()
        {
            // Arrange
            var context = InMemoryDbContextFactory.Create("TestDb_UpdateUser");
            await DatabaseSeeder.SeedUsers(context);
            var service = new UserService(context, _mapper, _configuration);

            var updateDTO = new UserUpdateDTO
            {
                Username = "UpdatedUser",
                Email = "updated@test.com",
                IsPublic = true
            };

            // Act
            var result = await service.UpdateUser(1, updateDTO);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<UserResponseDTO>();
            result.Username.Should().Be("UpdatedUser");
            result.Email.Should().Be("updated@test.com");
        }

        [Fact]
        public async Task UpdateUser_ShouldReturnNull_WhenUserDoesNotExist()
        {
            // Arrange
            var context = InMemoryDbContextFactory.Create("TestDb_UpdateUser_NonExistent");
            await DatabaseSeeder.SeedUsers(context);
            var service = new UserService(context, _mapper, _configuration);

            var updateDTO = new UserUpdateDTO
            {
                Username = "UpdatedUser",
                Email = "updated@test.com",
                IsPublic = true
            };

            // Act
            var result = await service.UpdateUser(99, updateDTO);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task DeleteUser_ShouldRemoveUser_WhenUserExists()
        {
            // Arrange
            var context = InMemoryDbContextFactory.Create("TestDb_DeleteUser");
            await DatabaseSeeder.SeedUsers(context);
            var service = new UserService(context, _mapper, _configuration);

            // Act
            var result = await service.DeleteUser(1);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task DeleteUser_ShouldReturnFalse_WhenUserDoesNotExist()
        {
            // Arrange
            var context = InMemoryDbContextFactory.Create("TestDb_DeleteUser_NonExistent");
            await DatabaseSeeder.SeedUsers(context);
            var service = new UserService(context, _mapper, _configuration);

            // Act
            var result = await service.DeleteUser(99);

            // Assert
            result.Should().BeFalse();
        }
    }
}
