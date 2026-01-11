using Xunit;
using FinancialTracking.Domain.Entities;
using System;

namespace FinancialTracking.Tests.Domain.Entities
{
    public class OperationTests
    {
        [Fact]
        public void CreateOperation_ValidData_ShouldCreateSuccessfully()
        {
            // Arrange
            var type = CategoryType.Income;
            var bankAccountId = Guid.NewGuid();
            var amount = 1000m;
            var categoryId = Guid.NewGuid();
            var date = DateTime.Now;
            var description = "Salary";

            // Act
            var operation = Operation.Create(type, bankAccountId, amount, categoryId, date, description);

            // Assert
            Assert.NotNull(operation);
            Assert.Equal(type, operation.Type);
            Assert.Equal(bankAccountId, operation.BankAccountId);
            Assert.Equal(amount, operation.Amount);
            Assert.Equal(categoryId, operation.CategoryId);
            Assert.Equal(date, operation.Date);
            Assert.Equal(description, operation.Description);
            Assert.NotEqual(Guid.Empty, operation.Id);
        }

        [Fact]
        public void CreateOperation_ZeroAmount_ShouldThrowArgumentException()
        {
            // Arrange
            var type = CategoryType.Income;
            var bankAccountId = Guid.NewGuid();
            var amount = 0m;
            var categoryId = Guid.NewGuid();
            var date = DateTime.Now;

            // Act & Assert
            Assert.Throws<ArgumentException>(() => 
                Operation.Create(type, bankAccountId, amount, categoryId, date));
        }

        [Fact]
        public void CreateOperation_NegativeAmount_ShouldThrowArgumentException()
        {
            // Arrange
            var type = CategoryType.Income;
            var bankAccountId = Guid.NewGuid();
            var amount = -100m;
            var categoryId = Guid.NewGuid();
            var date = DateTime.Now;

            // Act & Assert
            Assert.Throws<ArgumentException>(() => 
                Operation.Create(type, bankAccountId, amount, categoryId, date));
        }

        [Fact]
        public void CreateOperation_WithoutDescription_ShouldCreateSuccessfully()
        {
            // Arrange
            var type = CategoryType.Expense;
            var bankAccountId = Guid.NewGuid();
            var amount = 50m;
            var categoryId = Guid.NewGuid();
            var date = DateTime.Now;

            // Act
            var operation = Operation.Create(type, bankAccountId, amount, categoryId, date);

            // Assert
            Assert.NotNull(operation);
            Assert.Null(operation.Description);
        }
    }
}