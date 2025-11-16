public class CreateExpenseDto
{
    public decimal Amount { get; set; }
    public string? Description { get; set; }
    public string Date { get; set; } = string.Empty;
    public int CategoryId { get; set; }
}
