using LeaveFlowAPI.DTOs;
using LeaveFlowAPI.Models;
using LeaveFlowAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LeaveFlowAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ApprovalController : ControllerBase
    {
        private readonly IApprovalService _approvalService;

        public ApprovalController(IApprovalService approvalService)
        {
            _approvalService = approvalService;
        }

        private int GetUserId() =>
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        // 取得待審核清單（僅限 Manager / Admin）
        [HttpGet("pending")]
        [Authorize(Roles =$"{Roles.Manager},{Roles.Admin}")]
        public async Task<IActionResult> GetPending()
        {
            var result = await _approvalService.GetPendingFormsAsync(GetUserId());
            return Ok(result);
        }

        // 核准
        [HttpPut("{formId}/approve")]
        [Authorize(Roles = $"{Roles.Manager},{Roles.Admin}")]
        public async Task<IActionResult> Approve(int formId, ApprovalActionDto dto)
        {
            bool success = await _approvalService.ApproveAsync(formId, GetUserId(), dto);
            if (!success) return BadRequest(new { message = "核准失敗，表單不存在或無權限" });

            return Ok(new { message = "已核准" });
        }

        // 退回
        [HttpPut("{formId}/reject")]
        [Authorize(Roles = $"{Roles.Manager},{Roles.Admin}")]
        public async Task<IActionResult> Reject(int formId, ApprovalActionDto dto)
        {
            bool success = await _approvalService.RejectAsync(formId, GetUserId(), dto);
            if (!success)
                return BadRequest(new { message = "退回失敗，表單不存在或無權限" });

            return Ok(new { message = "已退回" });
        }

        // 查看審核紀錄（所有登入者皆可查詢）
        [HttpGet("{formId}/history")]
        public async Task<IActionResult> GetHistory(int formId)
        {
            var result = await _approvalService.GetFormHistoryAsync(formId);
            return Ok(result);
        }
    }
}
