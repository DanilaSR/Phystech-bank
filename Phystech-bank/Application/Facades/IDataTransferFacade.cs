using System.Collections.Generic;
using FinancialTracking.Application.Import;
using FinancialTracking.Domain.Entities;

namespace FinancialTracking.Application.Facades
{
    public interface IDataTransferFacade
    {
        string ExportToJson(IEnumerable<BankAccount> accounts, 
                           IEnumerable<Category> categories, 
                           IEnumerable<Operation> operations);
        
        string ExportToCsv(IEnumerable<BankAccount> accounts, 
                          IEnumerable<Category> categories, 
                          IEnumerable<Operation> operations);
        
        ImportResult ImportFromJson(string filePath);
        ImportResult ImportFromCsv(string filePath);
        
        void ExportToFile(string filePath, 
                         IEnumerable<BankAccount> accounts,
                         IEnumerable<Category> categories,
                         IEnumerable<Operation> operations);
    }
}