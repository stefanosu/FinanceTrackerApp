using FinanceTrackerAPI.FinanceTracker.API;
using FinanceTrackerAPI.FinanceTracker.Domain.Entities;

using Microsoft.EntityFrameworkCore;

namespace FinanceTrackerAPI.FinanceTracker.Data
{
    public class FinanceTrackerDbContext : DbContext
    {
        public FinanceTrackerDbContext(DbContextOptions<FinanceTrackerDbContext> options)
            : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Expense> Expenses { get; set; }
        public DbSet<ExpenseCategory> ExpenseCategories { get; set; }
        public DbSet<ExpenseSubCategory> ExpenseSubCategories { get; set; }
        public DbSet<ExpensePaymentMethod> ExpensePaymentMethods { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<RecurringTransaction> RecurringTransactions { get; set; }
        public DbSet<SavingsGoal> SavingsGoals { get; set; }
        public DbSet<Household> Households { get; set; }
        public DbSet<HouseholdMember> HouseholdMembers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Global query filters for soft delete
            // These automatically exclude deleted records from all queries
            // Use .IgnoreQueryFilters() to include deleted records when needed
            modelBuilder.Entity<User>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<Expense>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<Account>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<Transaction>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<RecurringTransaction>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<SavingsGoal>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<Household>().HasQueryFilter(e => !e.IsDeleted);

            // Household relationships
            modelBuilder.Entity<Household>()
                .HasOne(h => h.Owner)
                .WithMany()
                .HasForeignKey(h => h.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<HouseholdMember>()
                .HasOne(hm => hm.Household)
                .WithMany(h => h.Members)
                .HasForeignKey(hm => hm.HouseholdId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<HouseholdMember>()
                .HasOne(hm => hm.User)
                .WithMany()
                .HasForeignKey(hm => hm.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Unique constraint on invite code
            modelBuilder.Entity<Household>()
                .HasIndex(h => h.InviteCode)
                .IsUnique();

            // Expense to Household relationship
            modelBuilder.Entity<Expense>()
                .HasOne(e => e.Household)
                .WithMany()
                .HasForeignKey(e => e.HouseholdId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
