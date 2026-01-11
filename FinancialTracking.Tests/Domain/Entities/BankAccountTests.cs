using FinancialTracking.Domain.Entities;
using System;

namespace FinancialTracking.Tests.Domain.Entities
{
    public class BankAccountTests
    {
        [Fact]
        public void CreateAccount_ValidData_ShouldCreateSuccessfully()
        {
            // Arrange
            var name = "Test Account";
            var initialBalance = 1000m;

            // Act
            var account = BankAccount.Create(name, initialBalance);

            // Assert
            Assert.NotNull(account);
            Assert.Equal(name, account.Name);
            Assert.Equal(initialBalance, account.Balance);
            Assert.NotEqual(Guid.Empty, account.Id);
        }

        [Fact]
        public void CreateAccount_EmptyName_ShouldThrowArgumentException()
        {
            // Arrange
            var name = "";
            var initialBalance = 1000m;

            // Act & Assert
            Assert.Throws<ArgumentException>(() => BankAccount.Create(name, initialBalance));
        }

        [Fact]
        public void CreateAccount_NegativeBalance_ShouldThrowArgumentException()
        {
            // Arrange
            var name = "Test Account";
            var initialBalance = -100m;

            // Act & Assert
            Assert.Throws<ArgumentException>(() => BankAccount.Create(name, initialBalance));
        }

        [Fact]
        public void UpdateBalance_WithPositiveAmount_ShouldIncreaseBalance()
        {
            // Arrange
            var account = BankAccount.Create("Test", 1000m);
            var amount = 500m;

            // Act
            account.UpdateBalance(amount);

            // Assert
            Assert.Equal(1500m, account.Balance);
        }

        [Fact]
        public void UpdateBalance_WithNegativeAmount_ShouldDecreaseBalance()
        {
            // Arrange
            var account = BankAccount.Create("Test", 1000m);
            var amount = -300m;

            // Act
            account.UpdateBalance(amount);

            // Assert
            Assert.Equal(700m, account.Balance);
        }
    }
}