using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinanceTrackerAPI.Services.Dtos
{
    public class UpdateAccountDto
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? AccountType { get; set; }
        public string? Description { get; set; }
    }
}