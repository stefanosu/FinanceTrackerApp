using System;
using System.Collections.Generic;


public class Transaction
{
    public int Id { get; set; }
    public int AccountId { get; set; }
    public int Amount { get; set; }
    public required string Type { get; set; }
    public DateOnly dateOnly { get; set; } = new DateOnly();
    public int CategoryId { get; set; }
    public required string Notes { get; set; }

    public Transaction() { }

    public Transaction(int id, int accountId, int amount, string type, DateOnly dateOnly, int categoryId, string notes)
    {
        Id = id;
        AccountId = accountId;
        Amount = amount;
        Type = type;
        this.dateOnly = dateOnly;
        CategoryId = categoryId;
        Notes = notes;
    }
}
