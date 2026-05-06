using FluentAssertions;
using LeaveFlowAPI.Data;
using LeaveFlowAPI.DTOs;
using LeaveFlowAPI.Helpers;
using LeaveFlowAPI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;

namespace LeaveFlowAPI.Tests
{
    public class AuthServiceTests
    {
        private AppDbContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new AppDbContext(options);
        }

        private JwtHelper CreateJwtHelper()
        {
            var inMemorySettings = new Dictionary<string, string>
            {
                { "JwtSettings:Secret", "LeaveFlowAPI_SuperSecretKey_2024_MustBe32Chars!" },
                { "JwtSettings:Issuer", "LeaveFlowAPI" },
                { "JwtSettings:Audience", "LeaveFlowClient" },
                { "JwtSettings:ExpirationDays", "7" }
            };

            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            return new JwtHelper(config);
        }

        [Fact]
        public async Task Register_WithValidData_ShouldReturnToken()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var jwtHelper = CreateJwtHelper();
            var service = new AuthService(context, jwtHelper);

            var dto = new RegisterDto
            {
                Name = "測試員工",
                Email = "test@example.com",
                Password = "123456",
                Role = "Employee"
            };

            // Act
            var result = await service.RegisterAsync(dto);

            // Assert
            result.Should().NotBeNull();
            result!.Token.Should().NotBeNullOrEmpty();
            result.Email.Should().Be("test@example.com");
            result.Role.Should().Be("Employee");
        }

        [Fact]
        public async Task Register_WithDuplicateEmail_ShouldReturnNull()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var jwtHelper = CreateJwtHelper();
            var service = new AuthService(context, jwtHelper);

            var dto = new RegisterDto
            {
                Name = "測試員工",
                Email = "test@example.com",
                Password = "123456",
                Role = "Employee"
            };

            // Act
            await service.RegisterAsync(dto); // 第一次註冊
            var result = await service.RegisterAsync(dto); // 重複 Email

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task Login_WithCorrectCredentials_ShouldReturnToken()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var jwtHelper = CreateJwtHelper();
            var service = new AuthService(context, jwtHelper);

            await service.RegisterAsync(new RegisterDto
            {
                Name = "測試員工",
                Email = "test@example.com",
                Password = "123456",
                Role = "Employee"
            });

            // Act
            var result = await service.LoginAsync(new LoginDto
            {
                Email = "test@example.com",
                Password = "123456"
            });

            // Assert
            result.Should().NotBeNull();
            result!.Token.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task Login_WithWrongPassword_ShouldReturnNull()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var jwtHelper = CreateJwtHelper();
            var service = new AuthService(context, jwtHelper);

            await service.RegisterAsync(new RegisterDto
            {
                Name = "測試員工",
                Email = "test@example.com",
                Password = "123456",
                Role = "Employee"
            });

            // Act
            var result = await service.LoginAsync(new LoginDto
            {
                Email = "test@example.com",
                Password = "wrong_password"
            });

            // Assert
            result.Should().BeNull();
        }
    }
}