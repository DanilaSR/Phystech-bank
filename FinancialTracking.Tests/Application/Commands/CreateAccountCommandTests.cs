using Xunit;
using Moq;
using FinancialTracking.Application.Commands;
using FinancialTracking.Domain.Entities;
using FinancialTracking.Domain.Factories;
using FinancialTracking.Application.Interfaces;

namespace FinancialTracking.Tests.Application.Commands
{
    public class CreateAccountCommandTests
    {
        [Fact]
        public void Execute_ShouldCreateAndSaveAccount()
        {
            // Arrange
            var accountName = "Test Account";
            var initialBalance = 1000m;
            var account = BankAccount.Create(accountName, initialBalance);
            
            var repositoryMock = new Mock<IRepository<BankAccount>>();
            var factoryMock = new Mock<IDomainFactory>();
            
            factoryMock.Setup(f => f.CreateAccount(accountName, initialBalance)).Returns(account);
            
            var command = new CreateAccountCommand(repositoryMock.Object, factoryMock.Object, accountName, initialBalance);

            // Act
            var result = command.Execute();

            // Assert
            Assert.Equal(account, result);
            factoryMock.Verify(f => f.CreateAccount(accountName, initialBalance), Times.Once);
            repositoryMock.Verify(r => r.Add(account), Times.Once);
        }
    }
}