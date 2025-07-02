using System; 
using System.Collections.Generic;


public class Transaction {
    public int Id { get; set;}
    public int AccountId { get; set;}
    public int Amount { get; set;}
    public required string Type { get; set;}
    public DateOnly dateOnly{ get; set;} = new DateOnly();
    public int CategoryId { get; set;}
    public required string Notes { get; set;}
    
}