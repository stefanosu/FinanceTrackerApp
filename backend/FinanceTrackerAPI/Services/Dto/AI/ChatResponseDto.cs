namespace FinanceTrackerAPI.Services.Dto.AI
{
    public class ChatResponseDto
    {
        public string Message { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
