using Xunit;
using Moq;
using FinancialTracking.Application.Commands;
using FinancialTracking.Application.Commands.Decorators;
using FinancialTracking.Infrastructure.DependencyInjection;

namespace FinancialTracking.Tests.Application.Commands.Decorators
{
    public class LoggingCommandDecoratorTests
    {
        [Fact]
        public void Execute_ShouldLogAndCallDecoratedCommand()
        {
            // Arrange
            var expectedResult = "Test Result";
            var commandMock = new Mock<ICommand<string>>();
            var loggerMock = new Mock<ILogger>();
            
            commandMock.Setup(c => c.Execute()).Returns(expectedResult);
            
            var decorator = new LoggingCommandDecorator<string>(commandMock.Object, loggerMock.Object);

            // Act
            var result = decorator.Execute();

            // Assert
            Assert.Equal(expectedResult, result);
            commandMock.Verify(c => c.Execute(), Times.Once);
            loggerMock.Verify(l => l.Log(It.IsAny<string>()), Times.AtLeast(2));
        }
    }
}