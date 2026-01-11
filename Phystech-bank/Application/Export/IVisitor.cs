using System.Collections.Generic;
using FinancialTracking.Domain.Entities;

namespace FinancialTracking.Application.Export
{
    public interface IVisitor
    {
        void Visit(BankAccount account);
        void Visit(Category category);
        void Visit(Operation operation);
        string GetResult();  // Добавьте этот метод
    }
}