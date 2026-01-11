using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using FinancialTracking.Domain.Entities;

namespace FinancialTracking.Application.Import
{
    public class JsonImporter : DataImporter
    {
        protected override string ReadFile(string filePath)
        {
            return File.ReadAllText(filePath);
        }

        protected override ParsedData ParseData(string data)
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                
                var jsonData = JsonSerializer.Deserialize<JsonData>(data, options);
                var result = new ParsedData();

                if (jsonData?.Accounts != null)
                {
                    foreach (var account in jsonData.Accounts)
                    {
                        // Note: In real app, we might preserve the original ID
                        var acc = BankAccount.Create(account.Name, account.Balance);
                        result.Accounts.Add(acc);
                    }
                }

                if (jsonData?.Categories != null)
                {
                    foreach (var category in jsonData.Categories)
                    {
                        // Исправление: правильное преобразование строки в enum
                        if (Enum.TryParse<CategoryType>(category.Type, true, out var type))
                        {
                            result.Categories.Add(Category.Create(category.Name, type));
                        }
                        else
                        {
                            throw new InvalidDataException($"Invalid category type: {category.Type}");
                        }
                    }
                }

                if (jsonData?.Operations != null)
                {
                    foreach (var operation in jsonData.Operations)
                    {
                        result.Operations.Add(Operation.Create(
                            operation.Type, operation.BankAccountId, operation.Amount,
                            operation.CategoryId, operation.Date, operation.Description
                        ));
                    }
                }

                return result;
            }
            catch (JsonException ex)
            {
                throw new InvalidDataException($"Invalid JSON format: {ex.Message}");
            }
        }

        // Helper classes for JSON deserialization
        private class JsonData
        {
            public List<AccountData> Accounts { get; set; }
            public List<CategoryData> Categories { get; set; }
            public List<OperationData> Operations { get; set; }
        }

        private class AccountData
        {
            public string Name { get; set; }
            public decimal Balance { get; set; }
        }

        private class CategoryData
        {
            public string Name { get; set; }
            public string Type { get; set; }
        }

        private class OperationData
        {
            public CategoryType Type { get; set; }
            public Guid BankAccountId { get; set; }
            public decimal Amount { get; set; }
            public Guid CategoryId { get; set; }
            public DateTime Date { get; set; }
            public string Description { get; set; }
        }
    }
}