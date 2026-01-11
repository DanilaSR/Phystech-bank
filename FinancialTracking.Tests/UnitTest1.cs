using Xunit;
using FinancialTracking.Domain.Entities;

namespace FinancialTracking.Tests
{
    public class BasicTests
    {
        [Fact]
        public void BankAccount_Create_ShouldWork()
        {
            // Arrange & Act
            var account = BankAccount.Create("Test", 1000m);

            // Assert
            Assert.NotNull(account);
            Assert.Equal("Test", account.Name);
            Assert.Equal(1000m, account.Balance);
        }

        [Fact]
        public void Category_Create_ShouldWork()
        {
            // Arrange & Act
            var category = Category.Create("Food", CategoryType.Expense);

            // Assert
            Assert.NotNull(category);
            Assert.Equal("Food", category.Name);
            Assert.Equal(CategoryType.Expense, category.Type);
        }

        [Fact]
        public void Operation_Create_ShouldWork()
        {
            // Arrange & Act
            var operation = Operation.Create(
                CategoryType.Income,
                System.Guid.NewGuid(),
                1000m,
                System.Guid.NewGuid(),
                System.DateTime.Now,
                "Test"
            );

            // Assert
            Assert.NotNull(operation);
            Assert.Equal(CategoryType.Income, operation.Type);
            Assert.Equal(1000m, operation.Amount);
        }
    }
}