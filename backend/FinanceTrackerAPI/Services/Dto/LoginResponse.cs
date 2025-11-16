namespace FinanceTrackerAPI.Services.Dto
{
    public record LoginResponse
    {
        public required string AccessToken { get; init; }
        public required string RefreshToken { get; init; }
    }
}

