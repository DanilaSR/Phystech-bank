using System;
using System.Collections.Generic;
using System.IO;
using FinancialTracking.Domain.Entities;

namespace FinancialTracking.Application.Import
{
    public abstract class DataImporter
    {
        public ImportResult Import(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"File not found: {filePath}");

            var data = ReadFile(filePath);
            var parsedData = ParseData(data);
            ValidateData(parsedData);
            
            return new ImportResult
            {
                Accounts = parsedData.Accounts,
                Categories = parsedData.Categories,
                Operations = parsedData.Operations,
                Success = true,
                Message = $"Successfully imported {parsedData.Accounts.Count} accounts, " +
                         $"{parsedData.Categories.Count} categories, " +
                         $"{parsedData.Operations.Count} operations"
            };
        }

        protected abstract string ReadFile(string filePath);
        protected abstract ParsedData ParseData(string data);
        
        protected virtual void ValidateData(ParsedData parsedData)
        {
            // Validate accounts
            foreach (var account in parsedData.Accounts)
            {
                if (account.Balance < 0)
                    throw new InvalidDataException($"Account {account.Name} has negative balance");
            }

            // Validate operations
            foreach (var operation in parsedData.Operations)
            {
                if (operation.Amount <= 0)
                    throw new InvalidDataException($"Operation {operation.Id} has invalid amount");

                // Check if referenced account exists
                if (!parsedData.Accounts.Exists(a => a.Id == operation.BankAccountId))
                    throw new InvalidDataException($"Operation references non-existent account: {operation.BankAccountId}");

                // Check if referenced category exists
                if (!parsedData.Categories.Exists(c => c.Id == operation.CategoryId))
                    throw new InvalidDataException($"Operation references non-existent category: {operation.CategoryId}");
            }
        }
    }

    public class ParsedData
    {
        public List<BankAccount> Accounts { get; set; } = new();
        public List<Category> Categories { get; set; } = new();
        public List<Operation> Operations { get; set; } = new();
    }

    public class ImportResult
    {
        public List<BankAccount> Accounts { get; set; } = new();
        public List<Category> Categories { get; set; } = new();
        public List<Operation> Operations { get; set; } = new();
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}