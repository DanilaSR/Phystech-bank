using System.Collections.Generic;
using FinancialTracking.Domain.Entities;

namespace FinancialTracking.Application.Export
{
    public interface IVisitor
    {
        void Visit(BankAccount account);
        void Visit(Category category);
        void Visit(Operation operation);
    }

    public class JsonExportVisitor : IVisitor
    {
        private readonly List<object> _data = new();

        public void Visit(BankAccount account)
        {
            _data.Add(new
            {
                account.Id,
                account.Name,
                account.Balance,
                Type = "BankAccount"
            });
        }

        public void Visit(Category category)
        {
            _data.Add(new
            {
                category.Id,
                category.Name,
                Type = category.Type.ToString(),
                ObjectType = "Category"
            });
        }

        public void Visit(Operation operation)
        {
            _data.Add(new
            {
                operation.Id,
                operation.Type,
                operation.BankAccountId,
                operation.Amount,
                operation.Date,
                operation.Description,
                operation.CategoryId,
                ObjectType = "Operation"
            });
        }

        public string GetJson()
        {
            return System.Text.Json.JsonSerializer.Serialize(_data, new System.Text.Json.JsonSerializerOptions
            {
                WriteIndented = true
            });
        }
    }
}