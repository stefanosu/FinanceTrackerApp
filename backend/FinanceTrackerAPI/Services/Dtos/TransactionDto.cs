using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinanceTrackerAPI.Services.Dtos
{
    public class TransactionDto
    {
        public string TransactionId { get; set; }
        public string TransactionType { get; set; }

    }
}