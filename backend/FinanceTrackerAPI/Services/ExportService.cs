using System.Globalization;

using CsvHelper;
using CsvHelper.Configuration;

using FinanceTrackerAPI.FinanceTracker.Data;
using FinanceTrackerAPI.Services.Interfaces;

using Microsoft.EntityFrameworkCore;

using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace FinanceTrackerAPI.Services
{
    public class ExportService : IExportService
    {
        private readonly FinanceTrackerDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public ExportService(FinanceTrackerDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;

            // Configure QuestPDF license (Community license for open source)
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public async Task<byte[]> ExportTransactionsToCsvAsync(DateOnly? startDate, DateOnly? endDate)
        {
            var userId = _currentUserService.GetUserId()
                ?? throw new UnauthorizedAccessException("User not authenticated");

            var query = _context.Transactions
                .Where(t => !t.IsDeleted);

            if (startDate.HasValue)
                query = query.Where(t => t.Date >= startDate.Value);
            if (endDate.HasValue)
                query = query.Where(t => t.Date <= endDate.Value);

            var transactions = await query
                .OrderByDescending(t => t.Date)
                .Select(t => new TransactionExportRow
                {
                    Date = t.Date.ToString("yyyy-MM-dd"),
                    Type = t.Type,
                    Amount = t.Amount,
                    Notes = t.Notes
                })
                .ToListAsync();

            return GenerateCsv(transactions);
        }

        public async Task<byte[]> ExportTransactionsToPdfAsync(DateOnly? startDate, DateOnly? endDate)
        {
            var userId = _currentUserService.GetUserId()
                ?? throw new UnauthorizedAccessException("User not authenticated");

            var query = _context.Transactions
                .Where(t => !t.IsDeleted);

            if (startDate.HasValue)
                query = query.Where(t => t.Date >= startDate.Value);
            if (endDate.HasValue)
                query = query.Where(t => t.Date <= endDate.Value);

            var transactions = await query
                .OrderByDescending(t => t.Date)
                .Select(t => new TransactionExportRow
                {
                    Date = t.Date.ToString("yyyy-MM-dd"),
                    Type = t.Type,
                    Amount = t.Amount,
                    Notes = t.Notes
                })
                .ToListAsync();

            var dateRangeText = GetDateRangeText(startDate, endDate);

            return GenerateTransactionsPdf(transactions, dateRangeText);
        }

        public async Task<byte[]> ExportExpensesToCsvAsync(DateOnly? startDate, DateOnly? endDate)
        {
            var userId = _currentUserService.GetUserId()
                ?? throw new UnauthorizedAccessException("User not authenticated");

            var query = _context.Expenses
                .Where(e => e.UserId == userId && !e.IsDeleted);

            if (startDate.HasValue)
                query = query.Where(e => e.Date >= startDate.Value.ToDateTime(TimeOnly.MinValue));
            if (endDate.HasValue)
                query = query.Where(e => e.Date <= endDate.Value.ToDateTime(TimeOnly.MaxValue));

            var expenses = await query
                .OrderByDescending(e => e.Date)
                .Select(e => new ExpenseExportRow
                {
                    Date = e.Date.ToString("yyyy-MM-dd"),
                    Description = e.Description,
                    Amount = e.Amount,
                    Category = e.Category
                })
                .ToListAsync();

            return GenerateCsv(expenses);
        }

        public async Task<byte[]> ExportExpensesToPdfAsync(DateOnly? startDate, DateOnly? endDate)
        {
            var userId = _currentUserService.GetUserId()
                ?? throw new UnauthorizedAccessException("User not authenticated");

            var query = _context.Expenses
                .Where(e => e.UserId == userId && !e.IsDeleted);

            if (startDate.HasValue)
                query = query.Where(e => e.Date >= startDate.Value.ToDateTime(TimeOnly.MinValue));
            if (endDate.HasValue)
                query = query.Where(e => e.Date <= endDate.Value.ToDateTime(TimeOnly.MaxValue));

            var expenses = await query
                .OrderByDescending(e => e.Date)
                .Select(e => new ExpenseExportRow
                {
                    Date = e.Date.ToString("yyyy-MM-dd"),
                    Description = e.Description,
                    Amount = e.Amount,
                    Category = e.Category
                })
                .ToListAsync();

            var dateRangeText = GetDateRangeText(startDate, endDate);

            return GenerateExpensesPdf(expenses, dateRangeText);
        }

        private static byte[] GenerateCsv<T>(IEnumerable<T> records)
        {
            using var memoryStream = new MemoryStream();
            using var writer = new StreamWriter(memoryStream);
            using var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture));

            csv.WriteRecords(records);
            writer.Flush();

            return memoryStream.ToArray();
        }

        private static byte[] GenerateTransactionsPdf(List<TransactionExportRow> transactions, string dateRange)
        {
            var totalIncome = transactions.Where(t => t.Type == "Income").Sum(t => t.Amount);
            var totalExpense = transactions.Where(t => t.Type == "Expense").Sum(t => Math.Abs(t.Amount));
            var netAmount = totalIncome - totalExpense;

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(40);
                    page.DefaultTextStyle(x => x.FontSize(10));

                    page.Header().Column(column =>
                    {
                        column.Item().Text("Transaction Report")
                            .FontSize(20).Bold().FontColor(Colors.Blue.Darken2);
                        column.Item().Text(dateRange)
                            .FontSize(12).FontColor(Colors.Grey.Darken1);
                        column.Item().PaddingTop(10).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                    });

                    page.Content().PaddingVertical(20).Column(column =>
                    {
                        // Summary section
                        column.Item().Row(row =>
                        {
                            row.RelativeItem().Background(Colors.Green.Lighten4).Padding(10).Column(c =>
                            {
                                c.Item().Text("Total Income").FontSize(9).FontColor(Colors.Grey.Darken1);
                                c.Item().Text($"${totalIncome:N2}").FontSize(14).Bold().FontColor(Colors.Green.Darken2);
                            });
                            row.ConstantItem(10);
                            row.RelativeItem().Background(Colors.Red.Lighten4).Padding(10).Column(c =>
                            {
                                c.Item().Text("Total Expenses").FontSize(9).FontColor(Colors.Grey.Darken1);
                                c.Item().Text($"${totalExpense:N2}").FontSize(14).Bold().FontColor(Colors.Red.Darken2);
                            });
                            row.ConstantItem(10);
                            row.RelativeItem().Background(Colors.Blue.Lighten4).Padding(10).Column(c =>
                            {
                                c.Item().Text("Net Amount").FontSize(9).FontColor(Colors.Grey.Darken1);
                                c.Item().Text($"${netAmount:N2}").FontSize(14).Bold()
                                    .FontColor(netAmount >= 0 ? Colors.Green.Darken2 : Colors.Red.Darken2);
                            });
                        });

                        column.Item().PaddingTop(20);

                        // Transactions table
                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(80);
                                columns.ConstantColumn(70);
                                columns.ConstantColumn(90);
                                columns.RelativeColumn();
                            });

                            table.Header(header =>
                            {
                                header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Date").Bold();
                                header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Type").Bold();
                                header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Amount").Bold();
                                header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Notes").Bold();
                            });

                            foreach (var transaction in transactions)
                            {
                                var bgColor = transaction.Type == "Income" ? Colors.Green.Lighten5 : Colors.Red.Lighten5;

                                table.Cell().Background(bgColor).Padding(5).Text(transaction.Date);
                                table.Cell().Background(bgColor).Padding(5).Text(transaction.Type);
                                table.Cell().Background(bgColor).Padding(5).Text($"${Math.Abs(transaction.Amount):N2}");
                                table.Cell().Background(bgColor).Padding(5).Text(transaction.Notes ?? "");
                            }
                        });
                    });

                    page.Footer().AlignCenter().Text(text =>
                    {
                        text.Span("Generated on ");
                        text.Span(DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm UTC"));
                        text.Span(" | FinanceTracker");
                    });
                });
            });

            return document.GeneratePdf();
        }

        private static byte[] GenerateExpensesPdf(List<ExpenseExportRow> expenses, string dateRange)
        {
            var totalAmount = expenses.Sum(e => e.Amount);

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(40);
                    page.DefaultTextStyle(x => x.FontSize(10));

                    page.Header().Column(column =>
                    {
                        column.Item().Text("Expense Report")
                            .FontSize(20).Bold().FontColor(Colors.Blue.Darken2);
                        column.Item().Text(dateRange)
                            .FontSize(12).FontColor(Colors.Grey.Darken1);
                        column.Item().PaddingTop(10).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                    });

                    page.Content().PaddingVertical(20).Column(column =>
                    {
                        // Summary
                        column.Item().Background(Colors.Blue.Lighten4).Padding(15).Row(row =>
                        {
                            row.RelativeItem().Column(c =>
                            {
                                c.Item().Text("Total Expenses").FontSize(12).FontColor(Colors.Grey.Darken1);
                                c.Item().Text($"${totalAmount:N2}").FontSize(24).Bold().FontColor(Colors.Blue.Darken2);
                            });
                            row.RelativeItem().AlignRight().Column(c =>
                            {
                                c.Item().Text("Number of Expenses").FontSize(12).FontColor(Colors.Grey.Darken1);
                                c.Item().Text($"{expenses.Count}").FontSize(24).Bold().FontColor(Colors.Blue.Darken2);
                            });
                        });

                        column.Item().PaddingTop(20);

                        // Expenses table
                        column.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(80);
                                columns.RelativeColumn();
                                columns.ConstantColumn(90);
                                columns.ConstantColumn(100);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Date").Bold();
                                header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Description").Bold();
                                header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Amount").Bold();
                                header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Category").Bold();
                            });

                            var alternate = false;
                            foreach (var expense in expenses)
                            {
                                var bgColor = alternate ? Colors.Grey.Lighten5 : Colors.White;
                                alternate = !alternate;

                                table.Cell().Background(bgColor).Padding(5).Text(expense.Date);
                                table.Cell().Background(bgColor).Padding(5).Text(expense.Description);
                                table.Cell().Background(bgColor).Padding(5).Text($"${expense.Amount:N2}");
                                table.Cell().Background(bgColor).Padding(5).Text(expense.Category);
                            }
                        });
                    });

                    page.Footer().AlignCenter().Text(text =>
                    {
                        text.Span("Generated on ");
                        text.Span(DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm UTC"));
                        text.Span(" | FinanceTracker");
                    });
                });
            });

            return document.GeneratePdf();
        }

        private static string GetDateRangeText(DateOnly? startDate, DateOnly? endDate)
        {
            if (startDate.HasValue && endDate.HasValue)
                return $"{startDate.Value:MMM dd, yyyy} - {endDate.Value:MMM dd, yyyy}";
            if (startDate.HasValue)
                return $"From {startDate.Value:MMM dd, yyyy}";
            if (endDate.HasValue)
                return $"Until {endDate.Value:MMM dd, yyyy}";
            return "All Time";
        }

        // Export row classes
        private class TransactionExportRow
        {
            public string Date { get; set; } = string.Empty;
            public string Type { get; set; } = string.Empty;
            public decimal Amount { get; set; }
            public string? Notes { get; set; }
        }

        private class ExpenseExportRow
        {
            public string Date { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public decimal Amount { get; set; }
            public string Category { get; set; } = string.Empty;
        }
    }
}
