using System;

namespace FinanceTrackerAPI.FinanceTracker.Domain.Entities
{
    public class Category
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string CategoryType { get; set; }
        public required string Description { get; set; }

        public Category() { }

        public Category(int id, string name, string categoryType, string description)
        {
            Id = id;
            Name = name;
            CategoryType = categoryType;
            Description = description;
        }
    }
}
