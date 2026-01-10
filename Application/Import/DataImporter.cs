// Application/Import/DataImporter.cs
using System;
using System.Collections.Generic;
using System.IO;
using FinancialTracking.Domain.Entities;

namespace FinancialTracking.Application.Import
{
    public abstract class DataImporter
    {
        public void ImportData(string filePath)
        {
            var data = ReadFile(filePath);
            var operations = ParseData(data);
            ValidateData(operations);
            SaveData(operations);
            LogImport(operations.Count);
        }

        protected abstract string ReadFile(string filePath);
        protected abstract List<Operation> ParseData(string data);
        
        protected virtual void ValidateData(List<Operation> operations)
        {
            foreach (var operation in operations)
            {
                if (operation.Amount <= 0)
                    throw new InvalidDataException($"Invalid amount in operation {operation.Id}");
            }
        }

        protected virtual void SaveData(List<Operation> operations)
        {
            // Default implementation
            Console.WriteLine($"Saving {operations.Count} operations");
        }

        protected virtual void LogImport(int count)
        {
            Console.WriteLine($"Imported {count} operations successfully");
        }
    }

    public class JsonImporter : DataImporter
    {
        protected override string ReadFile(string filePath)
        {
            return File.ReadAllText(filePath);
        }

        protected override List<Operation> ParseData(string data)
        {
            // Simplified JSON parsing
            Console.WriteLine("Parsing JSON data...");
            return new List<Operation>();
        }
    }

    public class CsvImporter : DataImporter
    {
        protected override string ReadFile(string filePath)
        {
            return File.ReadAllText(filePath);
        }

        protected override List<Operation> ParseData(string data)
        {
            Console.WriteLine("Parsing CSV data...");
            return new List<Operation>();
        }
    }
}