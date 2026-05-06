using LeaveFlowAPI.DTOs;
using LeaveFlowAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LeaveFlowAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FormController : ControllerBase
    {
        private readonly IFormService _formService;

        public FormController(IFormService formService)
        {
            _formService = formService;
        }

        // 取得目前登入使用者的 ID
        private int GetUserId() =>
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [HttpPost]
        public async Task<IActionResult> Create(CreateFormDto dto)
        {
            var result = await _formService.CreateAsync(GetUserId(), dto);
            return Ok(result);
        }

        [HttpGet("my")]
        public async Task<IActionResult> GetMyForms()
        {
            var result = await _formService.GetMyFormsAsync(GetUserId());
            return Ok(result);
        }

        [HttpPut("{id}/cancel")]
        public async Task<IActionResult> Cancel(int id)
        {
            var success = await _formService.CancelAsync(id, GetUserId());
            if (!success)
                return BadRequest(new { message = "取消失敗，表單不存在或狀態不允許取消" });

            return Ok(new { message = "已成功取消申請" });
        }
    }
}