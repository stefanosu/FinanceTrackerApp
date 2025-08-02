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
        public required string AccountType { get; set; }
        // Add other fields as needed, e.g., AccountType, Currency, etc.
    }
}