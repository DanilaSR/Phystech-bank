using System;

namespace FinancialTracking.Domain.Entities
{
    public class BankAccount
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public decimal Balance { get; private set; }

        private BankAccount() { }

        public static BankAccount Create(string name, decimal initialBalance = 0)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Account name cannot be empty");

            if (initialBalance < 0)
                throw new ArgumentException("Initial balance cannot be negative");

            return new BankAccount
            {
                Id = Guid.NewGuid(),
                Name = name,
                Balance = initialBalance
            };
        }

        public void UpdateBalance(decimal amount)
        {
            Balance += amount;
        }
    }
}