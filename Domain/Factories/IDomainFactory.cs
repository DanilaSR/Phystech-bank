using System;
using FinancialTracking.Domain.Entities;

namespace FinancialTracking.Domain.Factories
{
    public interface IDomainFactory
    {
        BankAccount CreateAccount(string name, decimal initialBalance);
        Category CreateCategory(string name, CategoryType type);
        Operation CreateOperation(CategoryType type, Guid bankAccountId, decimal amount,
                                Guid categoryId, DateTime date, string? description);
    }

    public class DomainFactory : IDomainFactory
    {
        public BankAccount CreateAccount(string name, decimal initialBalance)
        {
            return BankAccount.Create(name, initialBalance);
        }

        public Category CreateCategory(string name, CategoryType type)
        {
            return Category.Create(name, type);
        }

        public Operation CreateOperation(CategoryType type, Guid bankAccountId, decimal amount,
                                       Guid categoryId, DateTime date, string? description)
        {
            return Operation.Create(type, bankAccountId, amount, categoryId, date, description);
        }
    }
}