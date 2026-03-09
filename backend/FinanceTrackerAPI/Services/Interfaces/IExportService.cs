namespace FinanceTrackerAPI.Services.Interfaces
{
    public interface IExportService
    {
        Task<byte[]> ExportTransactionsToCsvAsync(DateOnly? startDate, DateOnly? endDate);
        Task<byte[]> ExportTransactionsToPdfAsync(DateOnly? startDate, DateOnly? endDate);
        Task<byte[]> ExportExpensesToCsvAsync(DateOnly? startDate, DateOnly? endDate);
        Task<byte[]> ExportExpensesToPdfAsync(DateOnly? startDate, DateOnly? endDate);
    }
}
