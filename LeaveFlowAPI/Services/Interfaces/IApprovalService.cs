using LeaveFlowAPI.DTOs;

namespace LeaveFlowAPI.Services.Interfaces
{
    public interface IApprovalService
    {
        Task<List<ApprovalResponseDto>> GetPendingFormsAsync(int managerId);
        Task<bool> ApproveAsync(int formId, int managerId, ApprovalActionDto dto);
        Task<bool> RejectAsync(int formId, int managerId, ApprovalActionDto dto);
        Task<List<ApprovalRecordResponseDto>> GetFormHistoryAsync(int formId);
    }
}