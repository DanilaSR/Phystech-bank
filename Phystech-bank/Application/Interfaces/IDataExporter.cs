using System.Collections.Generic;
using FinancialTracking.Domain.Entities;

namespace FinancialTracking.Application.Interfaces
{
    public interface IDataExporter
    {
        string Export(IEnumerable<BankAccount> accounts, 
                     IEnumerable<Category> categories, 
                     IEnumerable<Operation> operations);
        string FileExtension { get; }
    }
}

public interface IDataImporter
{
    void Import(string filePath, 
                out List<BankAccount> accounts, 
                out List<Category> categories, 
                out List<Operation> operations);
    bool CanImport(string fileExtension);
}