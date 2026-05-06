using FluentAssertions;
using FluentAssertions.Common;
using LeaveFlowAPI.Data;
using LeaveFlowAPI.DTOs;
using LeaveFlowAPI.Models;
using LeaveFlowAPI.Models.Entities;
using LeaveFlowAPI.Services;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace LeaveFlowAPI.Tests
{
    public class FormServiceTests : IAsyncLifetime
    {
        private AppDbContext _context = null!;
        private FormService _service = null!;
        private User _user = null!;
        private User _user2 = null!;
        
        // InitializeAsync 會在每個測試方法執行前被呼叫
        // 可以 await，用來取代建構子的非同步初始化
        public async Task InitializeAsync()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new AppDbContext(options);
            _service = new FormService(_context);
            _user = await CreateTestUser(_context);
            _user2 = await CreateTestUser(_context); 
        }

        private static async Task<User> CreateTestUser(AppDbContext context, string role = "Employee")
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
            var dto = new CreateFormDto
            {
                Type = FormType.Leave,
                StartDate = DateTime.Today.AddDays(1),
                EndDate = DateTime.Today.AddDays(3),
                Reason = "家庭因素"
            };

            // Act
            var result = await _service.CreateAsync(_user.Id, dto);

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
           var dto = new CreateFormDto
            {
                Type = FormType.Leave,
                StartDate = DateTime.Today.AddDays(1),
                EndDate = DateTime.Today.AddDays(2),
                Reason = "測試"
            };

            await _service.CreateAsync(_user.Id, dto);
            await _service.CreateAsync(_user2.Id, dto);

            // Act
            var result = await _service.GetMyFormsAsync(_user.Id);

            // Assert
            result.Should().HaveCount(1);
        }

        [Fact]
        public async Task CancelForm_WithPendingStatus_ShouldReturnTrue()
        {
            // Arrange
            var form = await _service.CreateAsync(_user.Id, new CreateFormDto
            {
                Type = FormType.Leave,
                StartDate = DateTime.Today.AddDays(1),
                EndDate = DateTime.Today.AddDays(2),
                Reason = "測試取消"
            });

            // Act
            var result = await _service.CancelAsync(form.Id, _user.Id);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task CancelForm_WithWrongUser_ShouldReturnFalse()
        {
            // Arrange
            var form = await _service.CreateAsync(_user.Id, new CreateFormDto
            {
                Type = FormType.Leave,
                StartDate = DateTime.Today.AddDays(1),
                EndDate = DateTime.Today.AddDays(2),
                Reason = "測試"
            });

            // Act — 用 user2 取消 user1 的表單
            var result = await _service.CancelAsync(form.Id, _user2.Id);

            // Assert
            result.Should().BeFalse();
        }

        // DisposeAsync 會在每個測試方法執行後被呼叫，用來清理資源
        public async Task DisposeAsync()
        {
            await _context.DisposeAsync();
        }
    }
}