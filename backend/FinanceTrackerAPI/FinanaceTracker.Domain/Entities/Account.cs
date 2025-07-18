using System;
using System.Collections.Generic;


public class Account
{
    public required int id { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
    public string AccountType  { get; set; }


    public Account() { }

    public Account(int id, string name, string email, string accountType)
    {
        this.id = id;
        this.Name = name;
        this.Email = email;
        this.AccountType = accountType;
    }
}