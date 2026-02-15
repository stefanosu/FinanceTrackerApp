using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinanceTrackerAPI.Services.Dtos
{
    public class TransactionDto
    {
        public required string TransactionId { get; set; }
        public required string TransactionType { get; set; }
    }
}
