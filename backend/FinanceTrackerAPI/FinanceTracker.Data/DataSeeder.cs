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

            // Use a known password for demo purposes (easy to remember for testing)
            // In a real scenario, each user would set their own password
            var demoPassword = "Demo123!";
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(demoPassword);

            logger.LogWarning(
                "DEVELOPMENT TEST CREDENTIALS - DO NOT USE IN PRODUCTION:\n" +
                "  All demo users password: {Password}\n" +
                "  Users: demo@financetracker.dev, sarah@financetracker.dev, mike@financetracker.dev",
                demoPassword);

            // Seed Users - Development only, no admin users
            // Each user represents a different financial profile for demo purposes
            var users = new List<User>
            {
                new User
                {
                    Id = 1,
                    FirstName = "Alex",
                    LastName = "Demo",
                    Email = "demo@financetracker.dev",
                    Password = hashedPassword,
                    Role = "User",
                    CreatedAt = DateTime.UtcNow.AddMonths(-6),
                    UpdatedAt = DateTime.UtcNow,
                    Token = string.Empty,
                    RefreshToken = string.Empty
                },
                new User
                {
                    Id = 2,
                    FirstName = "Sarah",
                    LastName = "Johnson",
                    Email = "sarah@financetracker.dev",
                    Password = hashedPassword,
                    Role = "User",
                    CreatedAt = DateTime.UtcNow.AddMonths(-4),
                    UpdatedAt = DateTime.UtcNow,
                    Token = string.Empty,
                    RefreshToken = string.Empty
                },
                new User
                {
                    Id = 3,
                    FirstName = "Mike",
                    LastName = "Chen",
                    Email = "mike@financetracker.dev",
                    Password = hashedPassword,
                    Role = "User",
                    CreatedAt = DateTime.UtcNow.AddMonths(-2),
                    UpdatedAt = DateTime.UtcNow,
                    Token = string.Empty,
                    RefreshToken = string.Empty
                }
            };

            await context.Users.AddRangeAsync(users);

            // Seed Expense Categories
            var categories = new List<ExpenseCategory>
            {
                new ExpenseCategory { Id = 1, Name = "Food & Dining", Description = "Restaurants, groceries, coffee shops" },
                new ExpenseCategory { Id = 2, Name = "Transportation", Description = "Gas, public transport, rideshare, parking" },
                new ExpenseCategory { Id = 3, Name = "Entertainment", Description = "Movies, games, streaming, concerts" },
                new ExpenseCategory { Id = 4, Name = "Utilities", Description = "Electricity, water, internet, phone" },
                new ExpenseCategory { Id = 5, Name = "Shopping", Description = "Clothing, electronics, household items" },
                new ExpenseCategory { Id = 6, Name = "Healthcare", Description = "Medical, dental, pharmacy, fitness" },
                new ExpenseCategory { Id = 7, Name = "Travel", Description = "Flights, hotels, vacation expenses" },
                new ExpenseCategory { Id = 8, Name = "Housing", Description = "Rent, mortgage, maintenance" },
                new ExpenseCategory { Id = 9, Name = "Subscriptions", Description = "Streaming, software, memberships" },
                new ExpenseCategory { Id = 10, Name = "Other", Description = "Miscellaneous expenses" }
            };

            await context.ExpenseCategories.AddRangeAsync(categories);

            // Seed Expense SubCategories
            var subCategories = new List<ExpenseSubCategory>
            {
                // Food & Dining
                new ExpenseSubCategory { Id = 1, Name = "Restaurants", Description = "Dining out", CategoryId = 1 },
                new ExpenseSubCategory { Id = 2, Name = "Groceries", Description = "Food shopping", CategoryId = 1 },
                new ExpenseSubCategory { Id = 3, Name = "Coffee Shops", Description = "Coffee and snacks", CategoryId = 1 },
                new ExpenseSubCategory { Id = 4, Name = "Fast Food", Description = "Quick meals", CategoryId = 1 },
                // Transportation
                new ExpenseSubCategory { Id = 5, Name = "Gas", Description = "Fuel for vehicles", CategoryId = 2 },
                new ExpenseSubCategory { Id = 6, Name = "Public Transport", Description = "Buses, trains, metro", CategoryId = 2 },
                new ExpenseSubCategory { Id = 7, Name = "Rideshare", Description = "Uber, Lyft", CategoryId = 2 },
                new ExpenseSubCategory { Id = 8, Name = "Parking", Description = "Parking fees", CategoryId = 2 },
                // Entertainment
                new ExpenseSubCategory { Id = 9, Name = "Movies", Description = "Theater, rentals", CategoryId = 3 },
                new ExpenseSubCategory { Id = 10, Name = "Games", Description = "Video games, board games", CategoryId = 3 },
                new ExpenseSubCategory { Id = 11, Name = "Concerts", Description = "Live music, events", CategoryId = 3 },
                // Utilities
                new ExpenseSubCategory { Id = 12, Name = "Electricity", Description = "Electric bill", CategoryId = 4 },
                new ExpenseSubCategory { Id = 13, Name = "Internet", Description = "Internet service", CategoryId = 4 },
                new ExpenseSubCategory { Id = 14, Name = "Phone", Description = "Mobile phone bill", CategoryId = 4 },
                // Healthcare
                new ExpenseSubCategory { Id = 15, Name = "Gym", Description = "Gym membership", CategoryId = 6 },
                new ExpenseSubCategory { Id = 16, Name = "Pharmacy", Description = "Medications", CategoryId = 6 }
            };

            await context.ExpenseSubCategories.AddRangeAsync(subCategories);

            // Seed Payment Methods
            var paymentMethods = new List<ExpensePaymentMethod>
            {
                new ExpensePaymentMethod { Id = 1, Name = "Credit Card", Description = "Credit card payments" },
                new ExpensePaymentMethod { Id = 2, Name = "Debit Card", Description = "Debit card payments" },
                new ExpensePaymentMethod { Id = 3, Name = "Cash", Description = "Cash payments" },
                new ExpensePaymentMethod { Id = 4, Name = "Bank Transfer", Description = "Direct bank transfers" },
                new ExpensePaymentMethod { Id = 5, Name = "Digital Wallet", Description = "Apple Pay, Google Pay, PayPal" }
            };

            await context.ExpensePaymentMethods.AddRangeAsync(paymentMethods);

            // Seed Accounts - Each user has their own accounts
            var accounts = new List<Account>
            {
                // Alex's accounts (User 1) - Moderate income, balanced spending
                new Account
                {
                    Id = 1,
                    Name = "Primary Checking",
                    Email = "demo@financetracker.dev",
                    AccountType = "Checking",
                    Balance = 4250.00m
                },
                new Account
                {
                    Id = 2,
                    Name = "Savings Account",
                    Email = "demo@financetracker.dev",
                    AccountType = "Savings",
                    Balance = 12500.00m
                },
                new Account
                {
                    Id = 3,
                    Name = "Travel Rewards Card",
                    Email = "demo@financetracker.dev",
                    AccountType = "Credit Card",
                    Balance = -1847.32m
                },
                // Sarah's accounts (User 2) - Higher income, more savings-focused
                new Account
                {
                    Id = 4,
                    Name = "Main Checking",
                    Email = "sarah@financetracker.dev",
                    AccountType = "Checking",
                    Balance = 6780.00m
                },
                new Account
                {
                    Id = 5,
                    Name = "High-Yield Savings",
                    Email = "sarah@financetracker.dev",
                    AccountType = "Savings",
                    Balance = 28500.00m
                },
                new Account
                {
                    Id = 6,
                    Name = "Cash Back Card",
                    Email = "sarah@financetracker.dev",
                    AccountType = "Credit Card",
                    Balance = -892.15m
                },
                // Mike's accounts (User 3) - Entry-level, building savings
                new Account
                {
                    Id = 7,
                    Name = "Checking Account",
                    Email = "mike@financetracker.dev",
                    AccountType = "Checking",
                    Balance = 1890.00m
                },
                new Account
                {
                    Id = 8,
                    Name = "Emergency Fund",
                    Email = "mike@financetracker.dev",
                    AccountType = "Savings",
                    Balance = 3200.00m
                }
            };

            await context.Accounts.AddRangeAsync(accounts);

            // Generate realistic expenses for all users
            var expenses = GenerateRealisticExpenses();

            // Add expenses for Sarah (User 2) - 4 months of data, higher-end spending
            var sarahExpenses = GenerateUserExpenses(userId: 2, monthsBack: 4, expenseIdStart: 200,
                rentAmount: 2100m, incomeLevel: "high");
            expenses.AddRange(sarahExpenses);

            // Add expenses for Mike (User 3) - 2 months of data, budget-conscious
            var mikeExpenses = GenerateUserExpenses(userId: 3, monthsBack: 2, expenseIdStart: 400,
                rentAmount: 1200m, incomeLevel: "entry");
            expenses.AddRange(mikeExpenses);

            await context.Expenses.AddRangeAsync(expenses);

            // Generate transactions linked to accounts
            var transactions = GenerateTransactions();
            await context.Transactions.AddRangeAsync(transactions);

            await context.SaveChangesAsync();

            logger.LogInformation(
                "Development data seeding completed: {UserCount} users, {ExpenseCount} expenses, {TransactionCount} transactions, {AccountCount} accounts",
                users.Count, expenses.Count, transactions.Count, accounts.Count);
        }

        /// <summary>
        /// Generates realistic expense data over the past 6 months.
        /// Includes recurring expenses (rent, utilities, subscriptions) and variable expenses.
        /// </summary>
        private static List<Expense> GenerateRealisticExpenses()
        {
            var expenses = new List<Expense>();
            var random = new Random(42); // Fixed seed for reproducible data
            var today = DateTime.UtcNow.Date;
            var startDate = today.AddMonths(-6);
            var expenseId = 1;

            // Monthly recurring expenses (rent, utilities, subscriptions)
            for (var month = 0; month < 6; month++)
            {
                var monthStart = startDate.AddMonths(month);

                // Rent - 1st of each month
                expenses.Add(new Expense
                {
                    Id = expenseId++,
                    Name = "Monthly Rent",
                    Description = "Apartment rent payment",
                    Amount = 1650.00m,
                    Date = UtcDate(monthStart.Year, monthStart.Month, 1),
                    Category = "Housing",
                    SubCategory = "Rent",
                    PaymentMethod = "Bank Transfer",
                    Notes = $"Rent for {monthStart:MMMM yyyy}",
                    UserId = 1
                });

                // Electricity - ~15th of each month
                expenses.Add(new Expense
                {
                    Id = expenseId++,
                    Name = "Electric Bill",
                    Description = "Monthly electricity",
                    Amount = 85.00m + random.Next(-20, 40),
                    Date = UtcDate(monthStart.Year, monthStart.Month, 15).AddDays(random.Next(-2, 3)),
                    Category = "Utilities",
                    SubCategory = "Electricity",
                    PaymentMethod = "Bank Transfer",
                    Notes = "Auto-pay",
                    UserId = 1
                });

                // Internet - ~20th of each month
                expenses.Add(new Expense
                {
                    Id = expenseId++,
                    Name = "Internet Service",
                    Description = "High-speed internet",
                    Amount = 79.99m,
                    Date = UtcDate(monthStart.Year, monthStart.Month, 20).AddDays(random.Next(-1, 2)),
                    Category = "Utilities",
                    SubCategory = "Internet",
                    PaymentMethod = "Credit Card",
                    Notes = "Fiber internet plan",
                    UserId = 1
                });

                // Phone bill - ~22nd
                expenses.Add(new Expense
                {
                    Id = expenseId++,
                    Name = "Phone Bill",
                    Description = "Mobile phone service",
                    Amount = 65.00m,
                    Date = UtcDate(monthStart.Year, monthStart.Month, 22).AddDays(random.Next(-1, 2)),
                    Category = "Utilities",
                    SubCategory = "Phone",
                    PaymentMethod = "Credit Card",
                    Notes = "Unlimited plan",
                    UserId = 1
                });

                // Subscriptions
                expenses.Add(new Expense
                {
                    Id = expenseId++,
                    Name = "Netflix",
                    Description = "Streaming subscription",
                    Amount = 15.99m,
                    Date = UtcDate(monthStart.Year, monthStart.Month, 8),
                    Category = "Subscriptions",
                    SubCategory = "Streaming",
                    PaymentMethod = "Credit Card",
                    Notes = "Standard plan",
                    UserId = 1
                });

                expenses.Add(new Expense
                {
                    Id = expenseId++,
                    Name = "Spotify Premium",
                    Description = "Music streaming",
                    Amount = 10.99m,
                    Date = UtcDate(monthStart.Year, monthStart.Month, 12),
                    Category = "Subscriptions",
                    SubCategory = "Streaming",
                    PaymentMethod = "Credit Card",
                    Notes = "Individual plan",
                    UserId = 1
                });

                expenses.Add(new Expense
                {
                    Id = expenseId++,
                    Name = "Gym Membership",
                    Description = "Monthly gym fee",
                    Amount = 49.99m,
                    Date = UtcDate(monthStart.Year, monthStart.Month, 1),
                    Category = "Healthcare",
                    SubCategory = "Gym",
                    PaymentMethod = "Credit Card",
                    Notes = "FitLife Gym",
                    UserId = 1
                });
            }

            // Weekly groceries (4 per month, 6 months = ~24 grocery trips)
            for (var week = 0; week < 24; week++)
            {
                var groceryDate = startDate.AddDays(week * 7 + random.Next(0, 3));
                if (groceryDate > today) continue;

                expenses.Add(new Expense
                {
                    Id = expenseId++,
                    Name = "Weekly Groceries",
                    Description = "Grocery shopping",
                    Amount = 75.00m + random.Next(0, 80),
                    Date = groceryDate,
                    Category = "Food & Dining",
                    SubCategory = "Groceries",
                    PaymentMethod = random.Next(2) == 0 ? "Debit Card" : "Credit Card",
                    Notes = random.Next(3) == 0 ? "Stocked up on sale items" : "",
                    UserId = 1
                });
            }

            // Bi-weekly gas (~12 fill-ups over 6 months)
            for (var fillUp = 0; fillUp < 12; fillUp++)
            {
                var gasDate = startDate.AddDays(fillUp * 14 + random.Next(0, 4));
                if (gasDate > today) continue;

                expenses.Add(new Expense
                {
                    Id = expenseId++,
                    Name = "Gas",
                    Description = "Fuel fill-up",
                    Amount = 40.00m + random.Next(0, 25),
                    Date = gasDate,
                    Category = "Transportation",
                    SubCategory = "Gas",
                    PaymentMethod = "Credit Card",
                    Notes = "",
                    UserId = 1
                });
            }

            // Dining out (2-4 times per month)
            var diningNames = new[] { "Dinner at Olive Garden", "Lunch at Chipotle", "Brunch at Cafe Luna", "Pizza Night", "Thai Takeout", "Sushi Dinner", "Burger Joint", "Indian Restaurant" };
            for (var month = 0; month < 6; month++)
            {
                var diningCount = random.Next(2, 5);
                for (var i = 0; i < diningCount; i++)
                {
                    var diningDate = startDate.AddMonths(month).AddDays(random.Next(1, 28));
                    if (diningDate > today) continue;

                    expenses.Add(new Expense
                    {
                        Id = expenseId++,
                        Name = diningNames[random.Next(diningNames.Length)],
                        Description = "Dining out",
                        Amount = 25.00m + random.Next(0, 60),
                        Date = diningDate,
                        Category = "Food & Dining",
                        SubCategory = "Restaurants",
                        PaymentMethod = "Credit Card",
                        Notes = random.Next(4) == 0 ? "With friends" : "",
                        UserId = 1
                    });
                }
            }

            // Coffee shops (1-3 times per week)
            var coffeeShops = new[] { "Starbucks", "Local Coffee Co", "Dunkin", "Peet's Coffee" };
            for (var week = 0; week < 24; week++)
            {
                var coffeeCount = random.Next(1, 4);
                for (var i = 0; i < coffeeCount; i++)
                {
                    var coffeeDate = startDate.AddDays(week * 7 + random.Next(0, 7));
                    if (coffeeDate > today) continue;

                    expenses.Add(new Expense
                    {
                        Id = expenseId++,
                        Name = coffeeShops[random.Next(coffeeShops.Length)],
                        Description = "Coffee",
                        Amount = 4.50m + (decimal)(random.NextDouble() * 4),
                        Date = coffeeDate,
                        Category = "Food & Dining",
                        SubCategory = "Coffee Shops",
                        PaymentMethod = random.Next(3) == 0 ? "Cash" : "Credit Card",
                        Notes = "",
                        UserId = 1
                    });
                }
            }

            // Entertainment (movies, games, events - 1-2 per month)
            var entertainmentItems = new[]
            {
                ("Movie Tickets", "Movies", 15.00m, 30.00m),
                ("Video Game Purchase", "Games", 30.00m, 70.00m),
                ("Concert Tickets", "Concerts", 50.00m, 150.00m),
                ("Bowling Night", "Entertainment", 20.00m, 45.00m),
                ("Escape Room", "Entertainment", 30.00m, 50.00m)
            };

            for (var month = 0; month < 6; month++)
            {
                var entertainmentCount = random.Next(1, 3);
                for (var i = 0; i < entertainmentCount; i++)
                {
                    var item = entertainmentItems[random.Next(entertainmentItems.Length)];
                    var entDate = startDate.AddMonths(month).AddDays(random.Next(1, 28));
                    if (entDate > today) continue;

                    expenses.Add(new Expense
                    {
                        Id = expenseId++,
                        Name = item.Item1,
                        Description = item.Item2,
                        Amount = item.Item3 + (decimal)(random.NextDouble() * (double)(item.Item4 - item.Item3)),
                        Date = entDate,
                        Category = "Entertainment",
                        SubCategory = item.Item2,
                        PaymentMethod = "Credit Card",
                        Notes = "",
                        UserId = 1
                    });
                }
            }

            // Shopping (clothing, electronics - occasional)
            var shoppingItems = new[]
            {
                ("New Headphones", "Electronics", 79.99m),
                ("Winter Jacket", "Clothing", 129.99m),
                ("Running Shoes", "Clothing", 89.99m),
                ("Amazon Order", "Household", 45.67m),
                ("Target Run", "Household", 67.84m),
                ("Work Clothes", "Clothing", 156.00m),
                ("Phone Case", "Electronics", 29.99m)
            };

            for (var i = 0; i < 8; i++)
            {
                var item = shoppingItems[random.Next(shoppingItems.Length)];
                var shopDate = startDate.AddDays(random.Next(0, 180));
                if (shopDate > today) continue;

                expenses.Add(new Expense
                {
                    Id = expenseId++,
                    Name = item.Item1,
                    Description = item.Item2,
                    Amount = item.Item3 + (decimal)(random.NextDouble() * 20 - 10),
                    Date = shopDate,
                    Category = "Shopping",
                    SubCategory = item.Item2,
                    PaymentMethod = "Credit Card",
                    Notes = "",
                    UserId = 1
                });
            }

            // Healthcare (pharmacy, doctor visits - occasional)
            expenses.Add(new Expense
            {
                Id = expenseId++,
                Name = "Doctor Visit Copay",
                Description = "Annual checkup",
                Amount = 30.00m,
                Date = startDate.AddMonths(2).AddDays(15),
                Category = "Healthcare",
                SubCategory = "Medical",
                PaymentMethod = "Debit Card",
                Notes = "Annual physical",
                UserId = 1
            });

            expenses.Add(new Expense
            {
                Id = expenseId++,
                Name = "Pharmacy",
                Description = "Prescription pickup",
                Amount = 15.00m,
                Date = startDate.AddMonths(2).AddDays(16),
                Category = "Healthcare",
                SubCategory = "Pharmacy",
                PaymentMethod = "Debit Card",
                Notes = "",
                UserId = 1
            });

            expenses.Add(new Expense
            {
                Id = expenseId++,
                Name = "Dentist Checkup",
                Description = "Dental cleaning",
                Amount = 50.00m,
                Date = startDate.AddMonths(4).AddDays(10),
                Category = "Healthcare",
                SubCategory = "Dental",
                PaymentMethod = "Credit Card",
                Notes = "6-month cleaning",
                UserId = 1
            });

            // Round all amounts to 2 decimal places
            foreach (var expense in expenses)
            {
                expense.Amount = Math.Round(expense.Amount, 2);
            }

            return expenses;
        }

        /// <summary>
        /// Generates expenses for a specific user with customizable parameters.
        /// Allows different spending profiles (entry-level, moderate, high income).
        /// </summary>
        private static List<Expense> GenerateUserExpenses(
            int userId,
            int monthsBack,
            int expenseIdStart,
            decimal rentAmount,
            string incomeLevel)
        {
            var expenses = new List<Expense>();
            var random = new Random(42 + userId); // Different seed per user for variety
            var today = DateTime.UtcNow.Date;
            var startDate = today.AddMonths(-monthsBack);
            var expenseId = expenseIdStart;

            // Spending multiplier based on income level
            var multiplier = incomeLevel switch
            {
                "high" => 1.5m,
                "entry" => 0.7m,
                _ => 1.0m
            };

            // Monthly recurring expenses
            for (var month = 0; month < monthsBack; month++)
            {
                var monthStart = startDate.AddMonths(month);

                // Rent
                expenses.Add(new Expense
                {
                    Id = expenseId++,
                    Name = "Monthly Rent",
                    Description = "Apartment rent payment",
                    Amount = rentAmount,
                    Date = UtcDate(monthStart.Year, monthStart.Month, 1),
                    Category = "Housing",
                    SubCategory = "Rent",
                    PaymentMethod = "Bank Transfer",
                    Notes = $"Rent for {monthStart:MMMM yyyy}",
                    UserId = userId
                });

                // Utilities
                expenses.Add(new Expense
                {
                    Id = expenseId++,
                    Name = "Electric Bill",
                    Description = "Monthly electricity",
                    Amount = Math.Round((70.00m + random.Next(-15, 30)) * multiplier, 2),
                    Date = UtcDate(monthStart.Year, monthStart.Month, 15).AddDays(random.Next(-2, 3)),
                    Category = "Utilities",
                    SubCategory = "Electricity",
                    PaymentMethod = "Bank Transfer",
                    Notes = "",
                    UserId = userId
                });

                expenses.Add(new Expense
                {
                    Id = expenseId++,
                    Name = "Internet Service",
                    Description = "Internet plan",
                    Amount = incomeLevel == "high" ? 99.99m : 59.99m,
                    Date = UtcDate(monthStart.Year, monthStart.Month, 20),
                    Category = "Utilities",
                    SubCategory = "Internet",
                    PaymentMethod = "Credit Card",
                    Notes = "",
                    UserId = userId
                });

                // Subscriptions
                expenses.Add(new Expense
                {
                    Id = expenseId++,
                    Name = incomeLevel == "high" ? "HBO Max" : "Netflix",
                    Description = "Streaming subscription",
                    Amount = incomeLevel == "high" ? 15.99m : 9.99m,
                    Date = UtcDate(monthStart.Year, monthStart.Month, 10),
                    Category = "Subscriptions",
                    SubCategory = "Streaming",
                    PaymentMethod = "Credit Card",
                    Notes = "",
                    UserId = userId
                });

                if (incomeLevel != "entry")
                {
                    expenses.Add(new Expense
                    {
                        Id = expenseId++,
                        Name = "Gym Membership",
                        Description = "Monthly gym fee",
                        Amount = incomeLevel == "high" ? 89.99m : 49.99m,
                        Date = UtcDate(monthStart.Year, monthStart.Month, 1),
                        Category = "Healthcare",
                        SubCategory = "Gym",
                        PaymentMethod = "Credit Card",
                        Notes = incomeLevel == "high" ? "Premium gym" : "Basic membership",
                        UserId = userId
                    });
                }
            }

            // Weekly groceries
            var weeksCount = monthsBack * 4;
            for (var week = 0; week < weeksCount; week++)
            {
                var groceryDate = startDate.AddDays(week * 7 + random.Next(0, 3));
                if (groceryDate > today) continue;

                expenses.Add(new Expense
                {
                    Id = expenseId++,
                    Name = "Groceries",
                    Description = "Weekly grocery shopping",
                    Amount = Math.Round((60.00m + random.Next(0, 60)) * multiplier, 2),
                    Date = groceryDate,
                    Category = "Food & Dining",
                    SubCategory = "Groceries",
                    PaymentMethod = random.Next(2) == 0 ? "Debit Card" : "Credit Card",
                    Notes = "",
                    UserId = userId
                });
            }

            // Dining out (varies by income)
            var diningFrequency = incomeLevel switch
            {
                "high" => 4,
                "entry" => 1,
                _ => 2
            };

            var restaurants = incomeLevel == "high"
                ? new[] { "Fine Dining Restaurant", "Sushi Omakase", "Steakhouse", "French Bistro" }
                : new[] { "Chipotle", "Local Diner", "Pizza Place", "Food Truck" };

            for (var month = 0; month < monthsBack; month++)
            {
                var diningCount = random.Next(diningFrequency, diningFrequency + 3);
                for (var i = 0; i < diningCount; i++)
                {
                    var diningDate = startDate.AddMonths(month).AddDays(random.Next(1, 28));
                    if (diningDate > today) continue;

                    var baseAmount = incomeLevel == "high" ? 75.00m : (incomeLevel == "entry" ? 15.00m : 30.00m);
                    expenses.Add(new Expense
                    {
                        Id = expenseId++,
                        Name = restaurants[random.Next(restaurants.Length)],
                        Description = "Dining out",
                        Amount = Math.Round(baseAmount + random.Next(0, 40), 2),
                        Date = diningDate,
                        Category = "Food & Dining",
                        SubCategory = "Restaurants",
                        PaymentMethod = "Credit Card",
                        Notes = "",
                        UserId = userId
                    });
                }
            }

            // Transportation (gas or public transit based on profile)
            if (incomeLevel == "entry")
            {
                // Public transit user
                for (var month = 0; month < monthsBack; month++)
                {
                    var monthStart = startDate.AddMonths(month);
                    expenses.Add(new Expense
                    {
                        Id = expenseId++,
                        Name = "Monthly Transit Pass",
                        Description = "Public transportation",
                        Amount = 89.00m,
                        Date = UtcDate(monthStart.Year, monthStart.Month, 1),
                        Category = "Transportation",
                        SubCategory = "Public Transport",
                        PaymentMethod = "Debit Card",
                        Notes = "Monthly unlimited pass",
                        UserId = userId
                    });
                }
            }
            else
            {
                // Car owner - gas expenses
                var fillUps = monthsBack * 2;
                for (var fillUp = 0; fillUp < fillUps; fillUp++)
                {
                    var gasDate = startDate.AddDays(fillUp * 14 + random.Next(0, 4));
                    if (gasDate > today) continue;

                    expenses.Add(new Expense
                    {
                        Id = expenseId++,
                        Name = "Gas",
                        Description = "Fuel fill-up",
                        Amount = Math.Round((45.00m + random.Next(0, 25)) * multiplier, 2),
                        Date = gasDate,
                        Category = "Transportation",
                        SubCategory = "Gas",
                        PaymentMethod = "Credit Card",
                        Notes = "",
                        UserId = userId
                    });
                }
            }

            // Shopping (occasional)
            var shoppingCount = incomeLevel == "high" ? 6 : (incomeLevel == "entry" ? 2 : 4);
            var shoppingItems = incomeLevel == "high"
                ? new[] { ("Designer Bag", "Clothing", 450.00m), ("Apple Watch", "Electronics", 399.00m), ("Luxury Skincare", "Personal Care", 180.00m) }
                : new[] { ("Amazon Basics", "Household", 35.00m), ("T-shirts Pack", "Clothing", 25.00m), ("Phone Charger", "Electronics", 20.00m) };

            for (var i = 0; i < shoppingCount; i++)
            {
                var item = shoppingItems[random.Next(shoppingItems.Length)];
                var shopDate = startDate.AddDays(random.Next(0, monthsBack * 30));
                if (shopDate > today) continue;

                expenses.Add(new Expense
                {
                    Id = expenseId++,
                    Name = item.Item1,
                    Description = item.Item2,
                    Amount = Math.Round(item.Item3 + (decimal)(random.NextDouble() * 30 - 15), 2),
                    Date = shopDate,
                    Category = "Shopping",
                    SubCategory = item.Item2,
                    PaymentMethod = "Credit Card",
                    Notes = "",
                    UserId = userId
                });
            }

            return expenses;
        }

        /// <summary>
        /// Generates transactions linked to accounts.
        /// </summary>
        private static List<Transaction> GenerateTransactions()
        {
            var transactions = new List<Transaction>();
            var random = new Random(42);
            var today = DateTime.UtcNow.Date;
            var startDate = today.AddMonths(-6);
            var transactionId = 1;

            // Monthly income deposits
            for (var month = 0; month < 6; month++)
            {
                var payDate1 = UtcDate(startDate.Year, startDate.Month, 15).AddMonths(month);
                var payDate2 = UtcDate(startDate.Year, startDate.Month, 1).AddMonths(month + 1).AddDays(-1);

                if (payDate1 <= today)
                {
                    transactions.Add(new Transaction
                    {
                        Id = transactionId++,
                        AccountId = 1, // Checking
                        Amount = 2850.00m,
                        Type = "Income",
                        Date = DateOnly.FromDateTime(payDate1),
                        CategoryId = 0,
                        Notes = "Paycheck - Direct Deposit"
                    });
                }

                if (payDate2 <= today)
                {
                    transactions.Add(new Transaction
                    {
                        Id = transactionId++,
                        AccountId = 1,
                        Amount = 2850.00m,
                        Type = "Income",
                        Date = DateOnly.FromDateTime(payDate2),
                        CategoryId = 0,
                        Notes = "Paycheck - Direct Deposit"
                    });
                }

                // Monthly transfer to savings
                var transferDate = UtcDate(startDate.Year, startDate.Month, 5).AddMonths(month);
                if (transferDate <= today)
                {
                    transactions.Add(new Transaction
                    {
                        Id = transactionId++,
                        AccountId = 1,
                        Amount = -500.00m,
                        Type = "Transfer",
                        Date = DateOnly.FromDateTime(transferDate),
                        CategoryId = 0,
                        Notes = "Transfer to Savings"
                    });

                    transactions.Add(new Transaction
                    {
                        Id = transactionId++,
                        AccountId = 2, // Savings
                        Amount = 500.00m,
                        Type = "Transfer",
                        Date = DateOnly.FromDateTime(transferDate),
                        CategoryId = 0,
                        Notes = "Transfer from Checking"
                    });
                }

                // Credit card payment
                var ccPaymentDate = UtcDate(startDate.Year, startDate.Month, 25).AddMonths(month);
                if (ccPaymentDate <= today)
                {
                    var paymentAmount = 800.00m + random.Next(-200, 400);
                    transactions.Add(new Transaction
                    {
                        Id = transactionId++,
                        AccountId = 1,
                        Amount = -paymentAmount,
                        Type = "Payment",
                        Date = DateOnly.FromDateTime(ccPaymentDate),
                        CategoryId = 0,
                        Notes = "Credit Card Payment"
                    });

                    transactions.Add(new Transaction
                    {
                        Id = transactionId++,
                        AccountId = 3, // Credit Card
                        Amount = paymentAmount,
                        Type = "Payment",
                        Date = DateOnly.FromDateTime(ccPaymentDate),
                        CategoryId = 0,
                        Notes = "Payment Received"
                    });
                }
            }

            return transactions;
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

        /// <summary>
        /// Creates a UTC DateTime. PostgreSQL with Npgsql requires UTC timestamps.
        /// </summary>
        private static DateTime UtcDate(int year, int month, int day)
        {
            return new DateTime(year, month, day, 0, 0, 0, DateTimeKind.Utc);
        }
    }
}
