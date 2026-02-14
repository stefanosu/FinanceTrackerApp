namespace FinanceTrackerAPI.FinanceTracker.Domain.Entities
{
    /// <summary>
    /// Interface for entities that support soft delete.
    /// Instead of removing records from the database, they are marked as deleted.
    /// This preserves data integrity for analytics and audit trails.
    /// </summary>
    public interface ISoftDeletable
    {
        bool IsDeleted { get; set; }
        DateTime? DeletedAt { get; set; }
    }
}
