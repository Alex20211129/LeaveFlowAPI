using LeaveFlowAPI.Data;
using LeaveFlowAPI.DTOs;
using LeaveFlowAPI.Models;
using LeaveFlowAPI.Models.Entities;
using LeaveFlowAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LeaveFlowAPI.Services
{
    public class FormService : IFormService
    {
        private readonly AppDbContext _context;

        public FormService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<FormResponseDto> CreateAsync(int userId, CreateFormDto dto)
        {
            var form = new Form
            {
                UserId = userId,
                Type = dto.Type,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Reason = dto.Reason,
                Status = FormStatus.Pending
            };

            _context.Forms.Add(form);
            await _context.SaveChangesAsync();

            var user = await _context.Users.FindAsync(userId);

            return new FormResponseDto
            {
                Id = form.Id,
                ApplicantName = user!.Name,
                Type = form.Type,
                StartDate = form.StartDate,
                EndDate = form.EndDate,
                Reason = form.Reason,
                Status = form.Status,
                CreatedAt = form.CreatedAt
            };
        }

        public async Task<List<FormResponseDto>> GetMyFormsAsync(int userId)
        {
            return await _context.Forms
                .Include(f => f.User)
                .Where(f => f.UserId == userId)
                .OrderByDescending(f => f.CreatedAt)
                .Select(f => new FormResponseDto
                {
                    Id = f.Id,
                    ApplicantName = f.User.Name,
                    Type = f.Type,
                    StartDate = f.StartDate,
                    EndDate = f.EndDate,
                    Reason = f.Reason,
                    Status = f.Status,
                    CreatedAt = f.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<bool> CancelAsync(int formId, int userId)
        {
            var form = await _context.Forms
                .FirstOrDefaultAsync(f => f.Id == formId && f.UserId == userId);

            if (form == null) return false;

            // 只有 Pending 狀態才能取消
            if (form.Status != FormStatus.Pending) return false;

            form.Status = FormStatus.Cancelled;
            form.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}