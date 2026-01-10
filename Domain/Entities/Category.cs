// Domain/Entities/Category.cs
using System;

namespace FinancialTracking.Domain.Entities
{
    public enum CategoryType
    {
        Income,
        Expense
    }

    public class Category
    {
        public Guid Id { get; private set; }
        public CategoryType Type { get; private set; }
        public string Name { get; private set; }

        private Category() { }

        public static Category Create(string name, CategoryType type)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Category name cannot be empty");

            return new Category
            {
                Id = Guid.NewGuid(),
                Name = name,
                Type = type
            };
        }
    }
}