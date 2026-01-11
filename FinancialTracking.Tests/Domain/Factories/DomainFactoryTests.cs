using Xunit;
using Moq;
using FinancialTracking.Domain.Factories;
using FinancialTracking.Domain.Entities;
using System;

namespace FinancialTracking.Tests.Domain.Factories
{
    public class DomainFactoryTests
    {
        private readonly DomainFactory _factory;

        public DomainFactoryTests()
        {
            _factory = new DomainFactory();
        }

        [Fact]
        public void CreateAccount_ShouldReturnValidAccount()
        {
            // Arrange
            var name = "Test Account";
            var balance = 1000m;

            // Act
            var account = _factory.CreateAccount(name, balance);

            // Assert
            Assert.NotNull(account);
            Assert.Equal(name, account.Name);
            Assert.Equal(balance, account.Balance);
        }

        [Fact]
        public void CreateCategory_ShouldReturnValidCategory()
        {
            // Arrange
            var name = "Food";
            var type = CategoryType.Expense;

            // Act
            var category = _factory.CreateCategory(name, type);

            // Assert
            Assert.NotNull(category);
            Assert.Equal(name, category.Name);
            Assert.Equal(type, category.Type);
        }

        [Fact]
        public void CreateOperation_ShouldReturnValidOperation()
        {
            // Arrange
            var type = CategoryType.Income;
            var bankAccountId = Guid.NewGuid();
            var amount = 1000m;
            var categoryId = Guid.NewGuid();
            var date = DateTime.Now;
            var description = "Salary";

            // Act
            var operation = _factory.CreateOperation(type, bankAccountId, amount, categoryId, date, description);

            // Assert
            Assert.NotNull(operation);
            Assert.Equal(type, operation.Type);
            Assert.Equal(amount, operation.Amount);
            Assert.Equal(bankAccountId, operation.BankAccountId);
            Assert.Equal(categoryId, operation.CategoryId);
            Assert.Equal(date, operation.Date);
            Assert.Equal(description, operation.Description);
        }
    }
}