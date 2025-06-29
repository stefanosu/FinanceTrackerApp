using FinanceTrackerAPI.FinanceTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FinanceTrackerAPI.FinanceTracker.Data
{
    public static class DataSeeder
    {
        public static async Task SeedData(FinanceTrackerDbContext context)
        {
            // Check if data already exists
            if (await context.Users.AnyAsync())
            {
                return; // Data already seeded
            }

            // Seed Users
            var users = new List<User>
            {
                new User
                {
                    Id = 1,
                    FirstName = "John",
                    LastName = "Doe",
                    Email = "john.doe@example.com",
                    Password = "hashedpassword123",
                    Role = "User",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Token = "sample-token-1",
                    RefreshToken = "sample-refresh-token-1"
                },
                new User
                {
                    Id = 2,
                    FirstName = "Jane",
                    LastName = "Smith",
                    Email = "jane.smith@example.com",
                    Password = "hashedpassword456",
                    Role = "Admin",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Token = "sample-token-2",
                    RefreshToken = "sample-refresh-token-2"
                }
            };

            await context.Users.AddRangeAsync(users);

            // Seed Expense Categories
            var categories = new List<ExpenseCategory>
            {
                new ExpenseCategory { Id = 1, Name = "Food & Dining", Description = "Restaurants, groceries, etc." },
                new ExpenseCategory { Id = 2, Name = "Transportation", Description = "Gas, public transport, etc." },
                new ExpenseCategory { Id = 3, Name = "Entertainment", Description = "Movies, games, etc." },
                new ExpenseCategory { Id = 4, Name = "Utilities", Description = "Electricity, water, internet, etc." }
            };

            await context.ExpenseCategories.AddRangeAsync(categories);

            // Seed Expense SubCategories
            var subCategories = new List<ExpenseSubCategory>
            {
                new ExpenseSubCategory { Id = 1, Name = "Restaurants", Description = "Dining out", CategoryId = 1 },
                new ExpenseSubCategory { Id = 2, Name = "Groceries", Description = "Food shopping", CategoryId = 1 },
                new ExpenseSubCategory { Id = 3, Name = "Gas", Description = "Fuel for vehicles", CategoryId = 2 },
                new ExpenseSubCategory { Id = 4, Name = "Public Transport", Description = "Buses, trains, etc.", CategoryId = 2 }
            };

            await context.ExpenseSubCategories.AddRangeAsync(subCategories);

            // Seed Payment Methods
            var paymentMethods = new List<ExpensePaymentMethod>
            {
                new ExpensePaymentMethod { Id = 1, Name = "Credit Card", Description = "Credit card payments" },
                new ExpensePaymentMethod { Id = 2, Name = "Debit Card", Description = "Debit card payments" },
                new ExpensePaymentMethod { Id = 3, Name = "Cash", Description = "Cash payments" },
                new ExpensePaymentMethod { Id = 4, Name = "Bank Transfer", Description = "Direct bank transfers" }
            };

            await context.ExpensePaymentMethods.AddRangeAsync(paymentMethods);

            // Seed Sample Expenses
            var expenses = new List<Expense>
            {
                new Expense
                {
                    Id = 1,
                    Name = "Lunch at Restaurant",
                    Description = "Business lunch meeting",
                    Amount = 45.50m,
                    Date = DateTime.UtcNow.AddDays(-1),
                    Category = "Food & Dining",
                    SubCategory = "Restaurants",
                    PaymentMethod = "Credit Card",
                    Notes = "Client meeting",
                    UserId = 1
                },
                new Expense
                {
                    Id = 2,
                    Name = "Gas Station",
                    Description = "Fuel for car",
                    Amount = 65.00m,
                    Date = DateTime.UtcNow.AddDays(-2),
                    Category = "Transportation",
                    SubCategory = "Gas",
                    PaymentMethod = "Debit Card",
                    Notes = "Weekly fuel",
                    UserId = 1
                }
            };

            await context.Expenses.AddRangeAsync(expenses);

            await context.SaveChangesAsync();
        }
    }
} 