using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinanceTrackerAPI.Services.Dtos;

namespace FinanceTrackerAPI.Services.Interfaces
{
    public interface IUserService
    {
        Task<UserDto> CreateUserAsync(CreateUserDto dto);
        Task<UserDto> GetUserByIdAsync(int id);
        Task<UserDto> UpdateUserAsync(int id, CreateUserDto dto);
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        Task<bool> DeleteUserAsync(int id);
    }
}