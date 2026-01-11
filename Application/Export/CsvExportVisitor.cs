using System.Collections.Generic;
using System.Linq;
using System.Text;
using FinancialTracking.Domain.Entities;

namespace FinancialTracking.Application.Export
{
    public class CsvExportVisitor : IVisitor
    {
        private readonly StringBuilder _accountsCsv = new StringBuilder("Id,Name,Balance\n");
        private readonly StringBuilder _categoriesCsv = new StringBuilder("Id,Name,Type\n");
        private readonly StringBuilder _operationsCsv = new StringBuilder("Id,Type,BankAccountId,Amount,Date,Description,CategoryId\n");

        public void Visit(BankAccount account)
        {
            _accountsCsv.AppendLine($"{account.Id},{EscapeCsv(account.Name)},{account.Balance}");
        }

        public void Visit(Category category)
        {
            _categoriesCsv.AppendLine($"{category.Id},{EscapeCsv(category.Name)},{category.Type}");
        }

        public void Visit(Operation operation)
        {
            _operationsCsv.AppendLine($"{operation.Id},{operation.Type},{operation.BankAccountId}," +
                                     $"{operation.Amount},{operation.Date:yyyy-MM-dd HH:mm:ss}," +
                                     $"{EscapeCsv(operation.Description ?? "")},{operation.CategoryId}");
        }

        public string GetResult()
        {
            return $"=== Accounts ===\n{_accountsCsv}\n" +
                   $"=== Categories ===\n{_categoriesCsv}\n" +
                   $"=== Operations ===\n{_operationsCsv}";
        }

        private string EscapeCsv(string value)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;

            if (value.Contains(",") || value.Contains("\"") || value.Contains("\n"))
                return $"\"{value.Replace("\"", "\"\"")}\"";

            return value;
        }
    }
}