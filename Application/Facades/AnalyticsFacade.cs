using System;
using System.Collections.Generic;
using System.Linq;
using FinancialTracking.Domain.Entities;
using FinancialTracking.Application.Interfaces;

namespace FinancialTracking.Application.Facades
{
    public class AnalyticsFacade : IAnalyticsFacade
    {
        private readonly IRepository<Operation> _operationRepository;
        private readonly IRepository<Category> _categoryRepository;

        public AnalyticsFacade(IRepository<Operation> operationRepository, 
                              IRepository<Category> categoryRepository)
        {
            _operationRepository = operationRepository;
            _categoryRepository = categoryRepository;
        }

        public decimal CalculateBalanceDifference(DateTime startDate, DateTime endDate)
        {
            var operations = _operationRepository.GetAll()
                .Where(o => o.Date >= startDate && o.Date <= endDate);

            var totalIncome = operations
                .Where(o => o.Type == CategoryType.Income)
                .Sum(o => o.Amount);

            var totalExpenses = operations
                .Where(o => o.Type == CategoryType.Expense)
                .Sum(o => o.Amount);

            return totalIncome - totalExpenses;
        }

        public Dictionary<string, decimal> GroupByCategories(DateTime startDate, DateTime endDate)
        {
            var operations = _operationRepository.GetAll()
                .Where(o => o.Date >= startDate && o.Date <= endDate);

            var categories = _categoryRepository.GetAll()
                .ToDictionary(c => c.Id, c => c.Name);

            var result = new Dictionary<string, decimal>();

            foreach (var operation in operations)
            {
                var categoryName = categories.GetValueOrDefault(operation.CategoryId, "Unknown");
                if (!result.ContainsKey(categoryName))
                    result[categoryName] = 0;

                result[categoryName] += operation.Type == CategoryType.Income 
                    ? operation.Amount 
                    : -operation.Amount;
            }

            return result;
        }

        public decimal GetTotalIncome(DateTime startDate, DateTime endDate)
        {
            return _operationRepository.GetAll()
                .Where(o => o.Type == CategoryType.Income && 
                           o.Date >= startDate && o.Date <= endDate)
                .Sum(o => o.Amount);
        }

        public decimal GetTotalExpenses(DateTime startDate, DateTime endDate)
        {
            return _operationRepository.GetAll()
                .Where(o => o.Type == CategoryType.Expense && 
                           o.Date >= startDate && o.Date <= endDate)
                .Sum(o => o.Amount);
        }
    }
}