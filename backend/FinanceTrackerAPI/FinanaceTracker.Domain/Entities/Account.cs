using System;
using System.Collections.Generic;


public class Account
{
    public required int id { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }


    public Account() { }

    public Account(int id, string name, string email, string PasswordHash)
    {
        this.id = id;
        this.Name = name;
        this.Email = email;
        this.PasswordHash = PasswordHash;
    }
}