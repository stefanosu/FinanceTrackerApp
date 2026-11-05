namespace FinanceTrackerAPI.Services.Dtos
{
    public class HouseholdDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string InviteCode { get; set; } = string.Empty;
        public int OwnerId { get; set; }
        public string OwnerName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public List<HouseholdMemberDto> Members { get; set; } = new();
    }

    public class HouseholdMemberDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public DateTime JoinedAt { get; set; }
    }

    public class CreateHouseholdDto
    {
        public required string Name { get; set; }
    }

    public class UpdateHouseholdDto
    {
        public string? Name { get; set; }
    }

    public class JoinHouseholdDto
    {
        public required string InviteCode { get; set; }
    }

    public class HouseholdSummaryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int MemberCount { get; set; }
        public decimal TotalSharedExpenses { get; set; }
        public decimal ThisMonthSharedExpenses { get; set; }
    }
}
