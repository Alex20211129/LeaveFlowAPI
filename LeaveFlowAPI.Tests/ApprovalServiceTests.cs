using FluentAssertions;
using LeaveFlowAPI.Data;
using LeaveFlowAPI.DTOs;
using LeaveFlowAPI.Models;
using LeaveFlowAPI.Models.Entities;
using LeaveFlowAPI.Services;
using Microsoft.EntityFrameworkCore;

namespace LeaveFlowAPI.Tests
{
    public class ApprovalServiceTests
    {
        private AppDbContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new AppDbContext(options);
        }

        private async Task<(User manager, User employee, Form form)> SetupTestData(AppDbContext context)
        {
            var manager = new User
            {
                Name = "王主管",
                Email = "manager@test.com",
                PasswordHash = "hashed",
                Role = Roles.Manager
            };
            context.Users.Add(manager);
            await context.SaveChangesAsync();

            var employee = new User
            {
                Name = "李員工",
                Email = "employee@test.com",
                PasswordHash = "hashed",
                Role = Roles.Employee,
                ManagerId = manager.Id
            };
            context.Users.Add(employee);
            await context.SaveChangesAsync();

            var form = new Form
            {
                UserId = employee.Id,
                Type = FormType.Leave,
                StartDate = DateTime.Today.AddDays(1),
                EndDate = DateTime.Today.AddDays(3),
                Reason = "測試請假",
                Status = FormStatus.Pending
            };
            context.Forms.Add(form);
            await context.SaveChangesAsync();

            return (manager, employee, form);
        }

        [Fact]
        public async Task GetPendingForms_ShouldReturnSubordinateForms()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var service = new ApprovalService(context);
            var (manager, _, _) = await SetupTestData(context);

            // Act
            var result = await service.GetPendingFormsAsync(manager.Id);

            // Assert
            result.Should().HaveCount(1);
            result.First().ApplicantName.Should().Be("李員工");
        }

        [Fact]
        public async Task ApproveForm_ShouldChangeStatusToApproved()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var service = new ApprovalService(context);
            var (manager, _, form) = await SetupTestData(context);

            // Act
            var result = await service.ApproveAsync(form.Id, manager.Id, new ApprovalActionDto
            {
                Comment = "同意"
            });

            // Assert
            result.Should().BeTrue();
            var updatedForm = await context.Forms.FindAsync(form.Id);
            updatedForm!.Status.Should().Be(FormStatus.Approved);
        }

        [Fact]
        public async Task RejectForm_ShouldChangeStatusToRejected()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var service = new ApprovalService(context);
            var (manager, _, form) = await SetupTestData(context);

            // Act
            var result = await service.RejectAsync(form.Id, manager.Id, new ApprovalActionDto
            {
                Comment = "日期衝突，請重新申請"
            });

            // Assert
            result.Should().BeTrue();
            var updatedForm = await context.Forms.FindAsync(form.Id);
            updatedForm!.Status.Should().Be(FormStatus.Rejected);
        }

        [Fact]
        public async Task ApproveForm_WithWrongManager_ShouldReturnFalse()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var service = new ApprovalService(context);
            var (_, _, form) = await SetupTestData(context);

            // Act — 用不相關的主管 ID 審核
            var result = await service.ApproveAsync(form.Id, 999, new ApprovalActionDto
            {
                Comment = "不該成功"
            });

            // Assert
            result.Should().BeFalse();
        }
    }
}