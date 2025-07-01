using System; 
using System.Collections.Generic;


public class Account  {
    public required int id { get; set;} 
    public required string  Name { get; set; } 
    public required string  Email { get; set; } 
    public required string  PasswordHash { get; set; } 
}