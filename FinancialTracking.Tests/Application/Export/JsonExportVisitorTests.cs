using Xunit;
using FinancialTracking.Application.Export;
using FinancialTracking.Domain.Entities;
using System;
using System.Text.Json;

namespace FinancialTracking.Tests.Application.Export
{
    public class JsonExportVisitorTests
    {
        [Fact]
        public void VisitBankAccount_ShouldAddAccountToJson()
        {
            // Arrange
            var visitor = new JsonExportVisitor();
            var account = BankAccount.Create("Test Account", 1000m);

            // Act
            visitor.Visit(account);
            var result = visitor.GetResult();

            // Assert
            Assert.NotNull(result);
            Assert.Contains("Test Account", result);
            Assert.Contains("1000", result);
        }

        [Fact]
        public void VisitCategory_ShouldAddCategoryToJson()
        {
            // Arrange
            var visitor = new JsonExportVisitor();
            var category = Category.Create("Food", CategoryType.Expense);

            // Act
            visitor.Visit(category);
            var result = visitor.GetResult();

            // Assert
            Assert.NotNull(result);
            Assert.Contains("Food", result);
            Assert.Contains("Expense", result);
        }

        [Fact]
        public void VisitOperation_ShouldAddOperationToJson()
        {
            // Arrange
            var visitor = new JsonExportVisitor();
            var operation = Operation.Create(
                CategoryType.Income,
                Guid.NewGuid(),
                1000m,
                Guid.NewGuid(),
                DateTime.Now,
                "Salary"
            );

            // Act
            visitor.Visit(operation);
            var result = visitor.GetResult();

            // Assert
            Assert.NotNull(result);
            Assert.Contains("Income", result);
            Assert.Contains("1000", result);
            Assert.Contains("Salary", result);
        }

        [Fact]
        public void GetResult_ShouldReturnValidJson()
        {
            // Arrange
            var visitor = new JsonExportVisitor();
            var account = BankAccount.Create("Test", 1000m);
            var category = Category.Create("Food", CategoryType.Expense);

            // Act
            visitor.Visit(account);
            visitor.Visit(category);
            var result = visitor.GetResult();

            // Assert
            // Try to parse as JSON to validate structure
            var jsonDoc = JsonDocument.Parse(result);
            Assert.NotNull(jsonDoc);
        }
    }
}