namespace FinanceTrackerAPI.FinanceTracker.Domain.Entities
{
    public enum HouseholdRole
    {
        Member = 0,
        Owner = 1
    }

    public class Household : ISoftDeletable
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string InviteCode { get; set; }
        public required int OwnerId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public User? Owner { get; set; }
        public ICollection<HouseholdMember> Members { get; set; } = new List<HouseholdMember>();

        // Soft delete fields
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }

        public Household() { }
    }

    public class HouseholdMember
    {
        public int Id { get; set; }
        public int HouseholdId { get; set; }
        public int UserId { get; set; }
        public HouseholdRole Role { get; set; } = HouseholdRole.Member;
        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public Household? Household { get; set; }
        public User? User { get; set; }

        public HouseholdMember() { }
    }
}
