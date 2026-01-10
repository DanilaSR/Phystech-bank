using System;

namespace FinancialTracking.Domain.Entities
{
    public class Operation
    {
        public Guid Id { get; private set; }
        public CategoryType Type { get; private set; }
        public Guid BankAccountId { get; private set; }
        public decimal Amount { get; private set; }
        public DateTime Date { get; private set; }
        public string? Description { get; private set; }
        public Guid CategoryId { get; private set; }

        private Operation() { }

        public static Operation Create(CategoryType type, Guid bankAccountId, decimal amount, 
                                     Guid categoryId, DateTime date, string? description = null)
        {
            if (amount <= 0)
                throw new ArgumentException("Amount must be positive");

            return new Operation
            {
                Id = Guid.NewGuid(),
                Type = type,
                BankAccountId = bankAccountId,
                Amount = amount,
                CategoryId = categoryId,
                Date = date,
                Description = description
            };
        }
    }
}