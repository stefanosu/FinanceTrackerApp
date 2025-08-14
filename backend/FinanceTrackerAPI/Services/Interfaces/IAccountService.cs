using System.Collections.Generic;
using System.Threading.Tasks;
using FinanceTrackerAPI.Services.Dtos;

namespace FinanceTrackerAPI.Services.Interfaces
{
    public interface IAccountService
    {
        Task<AccountDto> CreateAccountAsync(CreateAccountDto dto);
        Task<AccountDto> GetAccountByIdAsync(int id);
        Task<AccountDto> UpdateAccountAsync(int id, UpdateAccountDto dto);
        Task<IEnumerable<AccountDto>> GetAllAccountsAsync();
        Task<bool> DeleteAccountAsync(int id);
    }
}