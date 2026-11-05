using FinanceTrackerAPI.Services.Dtos;

namespace FinanceTrackerAPI.Services.Interfaces
{
    public interface IHouseholdService
    {
        Task<HouseholdDto?> GetUserHouseholdAsync();
        Task<HouseholdDto> CreateHouseholdAsync(CreateHouseholdDto dto);
        Task<HouseholdDto> UpdateHouseholdAsync(int id, UpdateHouseholdDto dto);
        Task<bool> DeleteHouseholdAsync(int id);
        Task<HouseholdDto> JoinHouseholdAsync(JoinHouseholdDto dto);
        Task<bool> LeaveHouseholdAsync();
        Task<string> RegenerateInviteCodeAsync(int householdId);
        Task<bool> RemoveMemberAsync(int householdId, int userId);
        Task<HouseholdSummaryDto?> GetHouseholdSummaryAsync();
    }
}
