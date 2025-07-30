using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinanceTrackerAPI.Services.Dtos
{

    public class AccountDto
    {
        public required int Id { get; set; }
        public required string Name { get; set; }
        public required decimal Balance { get; set; }
        public required string Description { get; set; }
        public DateTime CreatedAt { get; set; }
<<<<<<< HEAD:backend/FinanceTrackerAPI/Services/Dto/AccountDto.cs
        public required string AccountType { get; set; }
        // Add other fields as needed, e.g., AccountType, Currency, etc.
=======
        public AccountType AccountType { get; set; }
        // Add other fields as needed, e.g., Currency, etc.

        public enum AccountType
        {
            Savings,
            Checking,
            Credit,
            Investment
        }
>>>>>>> 0363ffb9acfc04e1dbf24ec303c37eeab3dd2473:backend/FinanceTrackerAPI/Services/Dtos/AccountDto.cs
    }
}