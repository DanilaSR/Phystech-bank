using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using FinancialTracking.Domain.Entities;

namespace FinancialTracking.Application.Import
{
    public class CsvImporter : DataImporter
    {
        protected override string ReadFile(string filePath)
        {
            return File.ReadAllText(filePath);
        }

        protected override ParsedData ParseData(string data)
        {
            var result = new ParsedData();
            var lines = data.Split('\n', StringSplitOptions.RemoveEmptyEntries);

            var section = "";
            foreach (var line in lines)
            {
                if (line.StartsWith("==="))
                {
                    section = line.Replace("===", "").Trim();
                    continue;
                }

                if (line.StartsWith("Id,Name,Balance") && section == "Accounts")
                {
                    // Skip header
                    continue;
                }

                if (line.StartsWith("Id,Name,Type") && section == "Categories")
                {
                    // Skip header
                    continue;
                }

                if (line.StartsWith("Id,Type,BankAccountId,Amount,Date,Description,CategoryId") && section == "Operations")
                {
                    // Skip header
                    continue;
                }

                if (!string.IsNullOrEmpty(line.Trim()))
                {
                    ParseLine(line, section, result);
                }
            }

            return result;
        }

        private void ParseLine(string line, string section, ParsedData result)
        {
            var parts = ParseCsvLine(line);

            switch (section)
            {
                case "Accounts":
                    if (parts.Length >= 3)
                    {
                        // Note: We're ignoring the original ID from CSV and creating new accounts
                        var account = BankAccount.Create(
                            parts[1], 
                            decimal.Parse(parts[2], CultureInfo.InvariantCulture)
                        );
                        result.Accounts.Add(account);
                    }
                    break;

                case "Categories":
                    if (parts.Length >= 3)
                    {
                        var type = Enum.Parse<CategoryType>(parts[2], true);
                        var category = Category.Create(parts[1], type);
                        result.Categories.Add(category);
                    }
                    break;

                case "Operations":
                    if (parts.Length >= 7 && 
                        Guid.TryParse(parts[2], out var bankAccountId) &&
                        Guid.TryParse(parts[6], out var operationCategoryId))
                    {
                        var type = Enum.Parse<CategoryType>(parts[1], true);
                        var amount = decimal.Parse(parts[3], CultureInfo.InvariantCulture);
                        var date = DateTime.Parse(parts[4]);
                        var description = parts[5];

                        var operation = Operation.Create(
                            type, bankAccountId, amount,
                            operationCategoryId, date, description
                        );
                        result.Operations.Add(operation);
                    }
                    break;
            }
        }

        private string[] ParseCsvLine(string line)
        {
            var result = new List<string>();
            var current = "";
            var inQuotes = false;

            foreach (var ch in line)
            {
                if (ch == '"')
                {
                    inQuotes = !inQuotes;
                }
                else if (ch == ',' && !inQuotes)
                {
                    result.Add(current);
                    current = "";
                }
                else
                {
                    current += ch;
                }
            }

            result.Add(current);
            return result.ToArray();
        }
    }
}