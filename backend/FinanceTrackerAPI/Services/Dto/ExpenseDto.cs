using System.Text.Json.Serialization;

public class ExpenseDto
{
    public decimal Amount { get; set; }
    public string? Description { get; set; }
    [JsonPropertyName("expense_date")]
    public string Date { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
}
