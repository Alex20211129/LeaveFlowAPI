using LeaveFlowAPI.Data;
using LeaveFlowAPI.DTOs;
using LeaveFlowAPI.Models;
using LeaveFlowAPI.Models.Entities;
using LeaveFlowAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LeaveFlowAPI.Services
{
    public class ApprovalService : IApprovalService
    {
        private readonly AppDbContext _context;

        public ApprovalService(AppDbContext context)
        {
            _context = context;
        }

        //核准
        public async Task<bool> ApproveAsync(int formId, int managerId, ApprovalActionDto dto)
        {
            var form = await GetFormForManager(formId,managerId);
            if (form == null) return false;

            form.Status = FormStatus.Approved;
            form.UpdatedAt = DateTime.Now;

            _context.ApprovalRecords.Add(new ApprovalRecord
            {
                FormId = formId,
                ManagerId = managerId,
                Action = ApprovalAction.Approved,
                Comment = dto.Comment,
            });

            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> RejectAsync(int formId, int managerId, ApprovalActionDto dto)
        {
            var form = await GetFormForManager(formId, managerId);
            if (form == null) return false;

            form.Status = FormStatus.Rejected;
            form.UpdatedAt = DateTime.Now;

            _context.ApprovalRecords.Add(new ApprovalRecord
            {
                FormId = formId,
                ManagerId = managerId,
                Action = ApprovalAction.Rejected,
                Comment = dto.Comment
            });

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<ApprovalRecordResponseDto>> GetFormHistoryAsync(int formId)
        {
            return await _context.ApprovalRecords
                .Include(x=>x.Manager)
                .Where(x=>x.FormId == formId)
                .OrderByDescending(x=>x.ActionAt)
                .Select(x=> new ApprovalRecordResponseDto
                {
                    Id = x.Id,
                    ManagerName = x.Manager.Name,
                    Action = x.Action,
                    Comment = x.Comment,
                    ActionAt = x.ActionAt
                }).ToListAsync();
        }

        public async Task<List<ApprovalResponseDto>> GetPendingFormsAsync(int managerId)
        {
            return await _context.Forms
                .Include(f => f.User)
                .Where(f => f.User.ManagerId == managerId && f.Status == FormStatus.Pending)
                .OrderBy(f => f.CreatedAt)
                .Select(f => new ApprovalResponseDto
                {
                    FormId = f.Id,
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


        private async Task<Form?> GetFormForManager(int formId , int managerId)
        {
            return await _context.Forms.Include(f => f.User).FirstOrDefaultAsync(f=>f.Id == formId && f.User.ManagerId == managerId && f.Status == FormStatus.Pending);
        }
    }
}
