using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using FinancialTracking.Domain.Entities;

namespace FinancialTracking.Application.Export
{
    public class JsonExportVisitor : IVisitor
    {
        private readonly List<object> _accounts = new();
        private readonly List<object> _categories = new();
        private readonly List<object> _operations = new();

        public void Visit(BankAccount account)
        {
            _accounts.Add(new
            {
                account.Id,
                account.Name,
                account.Balance,
                Type = "BankAccount"
            });
        }

        public void Visit(Category category)
        {
            _categories.Add(new
            {
                category.Id,
                category.Name,
                Type = category.Type.ToString(),
                ObjectType = "Category"
            });
        }

        public void Visit(Operation operation)
        {
            _operations.Add(new
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

        public string GetResult()
        {
            var data = new
            {
                Accounts = _accounts,
                Categories = _categories,
                Operations = _operations,
                ExportDate = DateTime.UtcNow
            };

            return JsonSerializer.Serialize(data, new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
        }
    }
}