namespace FinanceTrackerAPI.Services.Dto.Analytics
{
    public class SpendingByCategoryDto
    {
        public string Category { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public decimal Percentage { get; set; }
    }
}
