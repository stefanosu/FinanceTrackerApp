using System.Security.Cryptography;

using FinanceTrackerAPI.FinanceTracker.Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FinanceTrackerAPI.FinanceTracker.Data
{
    /// <summary>
    /// Seeds initial data for development and testing.
    ///
    /// SECURITY CONSIDERATIONS:
    /// 1. Only seeds in Development environment
    /// 2. Generates random passwords (not stored in code)
    /// 3. Logs warnings when seeding occurs
    /// 4. Never seeds admin users by default
    /// </summary>
    public static class DataSeeder
    {
        public static async Task SeedData(
            FinanceTrackerDbContext context,
            IHostEnvironment environment,
            ILogger logger)
        {
            // SECURITY: Only seed in development
            if (!environment.IsDevelopment())
            {
                logger.LogInformation("Skipping data seeding in non-development environment");
                return;
            }

            // Check if data already exists
            if (await context.Users.AnyAsync())
            {
                return; // Data already seeded
            }

            logger.LogWarning(
                "DEVELOPMENT ONLY: Seeding test data. " +
                "This should never run in production!");

            // Generate random passwords for development users
            // These are logged to console for development testing ONLY
            var testPassword = GenerateRandomPassword();

            logger.LogWarning(
                "DEVELOPMENT TEST CREDENTIALS - DO NOT USE IN PRODUCTION:\n" +
                "  Email: dev.test@example.com\n" +
                "  Password: {Password}",
                testPassword);

            // Seed Users - Development only, no admin users
            var users = new List<User>
            {
                new User
                {
                    Id = 1,
                    FirstName = "Dev",
                    LastName = "Tester",
                    Email = "dev.test@example.com",
                    Password = BCrypt.Net.BCrypt.HashPassword(testPassword),
                    Role = "User", // Never seed admin users!
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Token = string.Empty,
                    RefreshToken = string.Empty
                }
            };

            await context.Users.AddRangeAsync(users);

            // Seed Expense Categories (safe, no sensitive data)
            var categories = new List<ExpenseCategory>
            {
                new ExpenseCategory { Id = 1, Name = "Food & Dining", Description = "Restaurants, groceries, etc." },
                new ExpenseCategory { Id = 2, Name = "Transportation", Description = "Gas, public transport, etc." },
                new ExpenseCategory { Id = 3, Name = "Entertainment", Description = "Movies, games, etc." },
                new ExpenseCategory { Id = 4, Name = "Utilities", Description = "Electricity, water, internet, etc." },
                new ExpenseCategory { Id = 5, Name = "Shopping", Description = "Retail purchases" },
                new ExpenseCategory { Id = 6, Name = "Healthcare", Description = "Medical expenses" },
                new ExpenseCategory { Id = 7, Name = "Travel", Description = "Vacation and trips" },
                new ExpenseCategory { Id = 8, Name = "Other", Description = "Miscellaneous expenses" }
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
                new ExpensePaymentMethod { Id = 4, Name = "Bank Transfer", Description = "Direct bank transfers" },
                new ExpensePaymentMethod { Id = 5, Name = "Digital Wallet", Description = "PayPal, Venmo, etc." }
            };

            await context.ExpensePaymentMethods.AddRangeAsync(paymentMethods);

            // Seed Sample Expenses (linked to test user)
            var expenses = new List<Expense>
            {
                new Expense
                {
                    Id = 1,
                    Name = "Sample Lunch",
                    Description = "Test expense for development",
                    Amount = 25.00m,
                    Date = DateTime.UtcNow.AddDays(-1),
                    Category = "Food & Dining",
                    SubCategory = "Restaurants",
                    PaymentMethod = "Credit Card",
                    Notes = "Development test data",
                    UserId = 1
                }
            };

            await context.Expenses.AddRangeAsync(expenses);

            await context.SaveChangesAsync();

            logger.LogInformation("Development data seeding completed successfully");
        }

        /// <summary>
        /// Generates a cryptographically secure random password.
        /// Used for development seeding only - passwords are not stored in code.
        /// </summary>
        private static string GenerateRandomPassword()
        {
            // Generate 16 bytes of random data
            var bytes = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(bytes);
            }

            // Convert to Base64 and add special char for complexity
            return Convert.ToBase64String(bytes) + "!";
        }
    }
}
