using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinanceTrackerAPI.Services.Dtos
{
    public class CreateAccountDto
    {
        // Add properties as needed, for example:
        public required string Name { get; set; }
        public required decimal InitialBalance { get; set; }
        public required string? Description { get; set; }
        public required int Id { get; set; }

    }
}