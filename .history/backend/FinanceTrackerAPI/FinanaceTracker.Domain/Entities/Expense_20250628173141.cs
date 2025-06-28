using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinanceTrackerAPI.FinanaceTracker.Domain.Entities
{
    public class Expense
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string Category { get; set; }
        public string SubCategory { get; set; }
        public string PaymentMethod { get; set; }
        public string Notes { get; set; }
    }
}