using System;
using System.Collections.Generic;
using FinancialTracking.Domain.Entities;

namespace FinancialTracking.Application.Facades
{
    public interface IAnalyticsFacade
    {
        decimal CalculateBalanceDifference(DateTime startDate, DateTime endDate);
        Dictionary<string, decimal> GroupByCategories(DateTime startDate, DateTime endDate);
        decimal GetTotalIncome(DateTime startDate, DateTime endDate);
        decimal GetTotalExpenses(DateTime startDate, DateTime endDate);
    }
}