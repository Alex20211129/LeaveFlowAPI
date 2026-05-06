using LeaveFlowAPI.DTOs;

namespace LeaveFlowAPI.Services.Interfaces
{
    public interface IFormService
    {
        Task<FormResponseDto> CreateAsync(int userId, CreateFormDto dto);
        Task<List<FormResponseDto>> GetMyFormsAsync(int userId);
        Task<bool> CancelAsync(int formId, int userId);
    }
}