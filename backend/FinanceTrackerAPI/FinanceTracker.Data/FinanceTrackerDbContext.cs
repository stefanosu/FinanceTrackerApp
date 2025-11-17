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
    }
}
