using FinanceTrackerAPI.Services.Dto;

namespace FinanceTrackerAPI.Services.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponse> LoginAsync(string email, string password);
    }
}

