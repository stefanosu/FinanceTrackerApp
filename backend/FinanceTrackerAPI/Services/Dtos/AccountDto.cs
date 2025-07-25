using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinanceTrackerAPI.Services.Dtos
{

    public class AccountDto
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public decimal Balance { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? AccountType { get; set; }
        // Add other fields as needed, e.g., AccountType, Currency, etc.
    }
}