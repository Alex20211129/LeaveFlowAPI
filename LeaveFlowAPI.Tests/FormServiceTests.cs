using FluentAssertions;
using LeaveFlowAPI.Data;
using LeaveFlowAPI.DTOs;
using LeaveFlowAPI.Models;
using LeaveFlowAPI.Models.Entities;
using LeaveFlowAPI.Services;
using Microsoft.EntityFrameworkCore;

namespace LeaveFlowAPI.Tests
{
    public class FormServiceTests
    {
        private AppDbContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new AppDbContext(options);
        }

        private async Task<User> CreateTestUser(AppDbContext context, string role = "Employee")
        {
            var user = new User
            {
                Name = "測試使用者",
                Email = $"{Guid.NewGuid()}@test.com",
                PasswordHash = "hashed",
                Role = role
            };
            context.Users.Add(user);
            await context.SaveChangesAsync();
            return user;
        }

        [Fact]
        public async Task CreateForm_ShouldReturnFormWithPendingStatus()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var service = new FormService(context);
            var user = await CreateTestUser(context);

            var dto = new CreateFormDto
            {
                Type = FormType.Leave,
                StartDate = DateTime.Today.AddDays(1),
                EndDate = DateTime.Today.AddDays(3),
                Reason = "家庭因素"
            };

            // Act
            var result = await service.CreateAsync(user.Id, dto);

            // Assert
            result.Should().NotBeNull();
            result.Status.Should().Be(FormStatus.Pending);
            result.ApplicantName.Should().Be("測試使用者");
            result.Type.Should().Be(FormType.Leave);
        }

        [Fact]
        public async Task GetMyForms_ShouldReturnOnlyCurrentUserForms()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var service = new FormService(context);
            var user1 = await CreateTestUser(context);
            var user2 = await CreateTestUser(context);

            var dto = new CreateFormDto
            {
                Type = FormType.Leave,
                StartDate = DateTime.Today.AddDays(1),
                EndDate = DateTime.Today.AddDays(2),
                Reason = "測試"
            };

            await service.CreateAsync(user1.Id, dto);
            await service.CreateAsync(user2.Id, dto);

            // Act
            var result = await service.GetMyFormsAsync(user1.Id);

            // Assert
            result.Should().HaveCount(1);
        }

        [Fact]
        public async Task CancelForm_WithPendingStatus_ShouldReturnTrue()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var service = new FormService(context);
            var user = await CreateTestUser(context);

            var form = await service.CreateAsync(user.Id, new CreateFormDto
            {
                Type = FormType.Leave,
                StartDate = DateTime.Today.AddDays(1),
                EndDate = DateTime.Today.AddDays(2),
                Reason = "測試取消"
            });

            // Act
            var result = await service.CancelAsync(form.Id, user.Id);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task CancelForm_WithWrongUser_ShouldReturnFalse()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var service = new FormService(context);
            var user1 = await CreateTestUser(context);
            var user2 = await CreateTestUser(context);

            var form = await service.CreateAsync(user1.Id, new CreateFormDto
            {
                Type = FormType.Leave,
                StartDate = DateTime.Today.AddDays(1),
                EndDate = DateTime.Today.AddDays(2),
                Reason = "測試"
            });

            // Act — 用 user2 取消 user1 的表單
            var result = await service.CancelAsync(form.Id, user2.Id);

            // Assert
            result.Should().BeFalse();
        }
    }
}