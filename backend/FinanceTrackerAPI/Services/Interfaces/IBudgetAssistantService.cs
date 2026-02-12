using FinanceTrackerAPI.Services.Dto.AI;

namespace FinanceTrackerAPI.Services.Interfaces
{
    public interface IBudgetAssistantService
    {
        Task<ChatResponseDto> GetChatResponseAsync(string message);
        Task<FinancialContextDto> GetFinancialContextAsync();
    }
}
