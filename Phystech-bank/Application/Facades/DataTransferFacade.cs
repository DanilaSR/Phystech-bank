using System;
using System.Collections.Generic;
using System.IO;
using FinancialTracking.Application.Export;  // Добавьте эту директиву
using FinancialTracking.Application.Import;
using FinancialTracking.Domain.Entities;

namespace FinancialTracking.Application.Facades
{
    public class DataTransferFacade : IDataTransferFacade
    {
        private readonly JsonImporter _jsonImporter;
        private readonly CsvImporter _csvImporter;

        public DataTransferFacade()
        {
            _jsonImporter = new JsonImporter();
            _csvImporter = new CsvImporter();
        }

        public string ExportToJson(IEnumerable<BankAccount> accounts, 
                                  IEnumerable<Category> categories, 
                                  IEnumerable<Operation> operations)
        {
            var visitor = new JsonExportVisitor();
            return ExportData(visitor, accounts, categories, operations);
        }

        public string ExportToCsv(IEnumerable<BankAccount> accounts, 
                                 IEnumerable<Category> categories, 
                                 IEnumerable<Operation> operations)
        {
            var visitor = new CsvExportVisitor();
            return ExportData(visitor, accounts, categories, operations);
        }

        public ImportResult ImportFromJson(string filePath)
        {
            return _jsonImporter.Import(filePath);
        }

        public ImportResult ImportFromCsv(string filePath)
        {
            return _csvImporter.Import(filePath);
        }

        public void ExportToFile(string filePath, 
                                IEnumerable<BankAccount> accounts,
                                IEnumerable<Category> categories,
                                IEnumerable<Operation> operations)
        {
            string content;
            string extension = Path.GetExtension(filePath).ToLower();

            switch (extension)
            {
                case ".json":
                    content = ExportToJson(accounts, categories, operations);
                    break;
                case ".csv":
                    content = ExportToCsv(accounts, categories, operations);
                    break;
                default:
                    throw new NotSupportedException($"Unsupported file format: {extension}");
            }

            File.WriteAllText(filePath, content);
        }

        private string ExportData(IVisitor visitor, 
                                 IEnumerable<BankAccount> accounts,
                                 IEnumerable<Category> categories,
                                 IEnumerable<Operation> operations)
        {
            foreach (var account in accounts)
                visitor.Visit(account);

            foreach (var category in categories)
                visitor.Visit(category);

            foreach (var operation in operations)
                visitor.Visit(operation);

            return visitor.GetResult();
        }
    }
}