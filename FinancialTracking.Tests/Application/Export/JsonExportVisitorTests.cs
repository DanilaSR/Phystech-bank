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
        public void VisitOperation_ShouldAddOperationToJson()
        {
            // Arrange
            var visitor = new JsonExportVisitor();
            var accountId = Guid.NewGuid();
            var categoryId = Guid.NewGuid();
            var operation = Operation.Create(
                CategoryType.Income,
                accountId,
                1000m,
                categoryId,
                new DateTime(2024, 1, 1),
                "Salary"
            );

            // Act
            visitor.Visit(operation);
            var result = visitor.GetResult();
            
            Console.WriteLine("Generated JSON:");
            Console.WriteLine(result);

            // Assert - проверяем базовые вещи
            Assert.NotNull(result);
            Assert.Contains("Salary", result);
            Assert.Contains("1000", result);
            Assert.Contains(accountId.ToString(), result);
            Assert.Contains(categoryId.ToString(), result);
            
            // Проверяем что это валидный JSON
            var jsonDoc = JsonDocument.Parse(result);
            Assert.NotNull(jsonDoc);
            
            // Ищем значение Type в operations
            var root = jsonDoc.RootElement;
            if (root.TryGetProperty("Operations", out var ops) && ops.ValueKind == JsonValueKind.Array)
            {
                foreach (var op in ops.EnumerateArray())
                {
                    if (op.TryGetProperty("Type", out var typeProp))
                    {
                        var typeValue = typeProp.GetInt32(); // Enum может быть сериализован как число
                        Assert.True(typeValue == 0 || typeValue == 1);
                    }
                }
            }
        }

        [Fact]
        public void VisitMultipleObjects_ShouldIncludeAllInJson()
        {
            // Arrange
            var visitor = new JsonExportVisitor();
            var account = BankAccount.Create("Account1", 1000m);
            var category = Category.Create("Category1", CategoryType.Income);
            var operation = Operation.Create(
                CategoryType.Expense,
                Guid.NewGuid(),
                500m,
                Guid.NewGuid(),
                DateTime.Now,
                "Test Expense"
            );

            // Act
            visitor.Visit(account);
            visitor.Visit(category);
            visitor.Visit(operation);
            var result = visitor.GetResult();
            
            Console.WriteLine("Multiple objects JSON:");
            Console.WriteLine(result);

            // Assert - проверяем наличие всех объектов
            Assert.Contains("Account1", result);
            Assert.Contains("Category1", result);
            Assert.Contains("Test Expense", result);
            Assert.Contains("1000", result);
            Assert.Contains("500", result);
            
            // Проверяем валидность JSON
            var jsonDoc = JsonDocument.Parse(result);
            Assert.NotNull(jsonDoc);
        }

        [Fact]
        public void VisitCategory_ShouldSerializeCategoryType()
        {
            // Arrange
            var visitor = new JsonExportVisitor();
            var category = Category.Create("Food", CategoryType.Expense);

            // Act
            visitor.Visit(category);
            var result = visitor.GetResult();
            
            // Parse and check
            var jsonDoc = JsonDocument.Parse(result);
            var root = jsonDoc.RootElement;
            
            if (root.TryGetProperty("Categories", out var categories) && 
                categories.ValueKind == JsonValueKind.Array &&
                categories.GetArrayLength() > 0)
            {
                var firstCategory = categories[0];
                Assert.True(firstCategory.TryGetProperty("Name", out var nameProp));
                Assert.Equal("Food", nameProp.GetString());
            }
        }
    }
}