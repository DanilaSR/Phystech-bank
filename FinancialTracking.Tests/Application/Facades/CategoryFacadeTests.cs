using Xunit;
using Moq;
using FinancialTracking.Application.Facades;
using FinancialTracking.Domain.Entities;
using FinancialTracking.Domain.Factories;
using FinancialTracking.Application.Interfaces;
using System;
using System.Linq;

namespace FinancialTracking.Tests.Application.Facades
{
    public class CategoryFacadeTests
    {
        private readonly Mock<IRepository<Category>> _repositoryMock;
        private readonly Mock<IDomainFactory> _factoryMock;
        private readonly CategoryFacade _facade;

        public CategoryFacadeTests()
        {
            _repositoryMock = new Mock<IRepository<Category>>();
            _factoryMock = new Mock<IDomainFactory>();
            _facade = new CategoryFacade(_repositoryMock.Object, _factoryMock.Object);
        }

        [Fact]
        public void CreateCategory_ShouldCreateAndSaveCategory()
        {
            // Arrange
            var name = "Food";
            var type = CategoryType.Expense;
            var category = Category.Create(name, type);
            
            _factoryMock.Setup(f => f.CreateCategory(name, type)).Returns(category);

            // Act
            var result = _facade.CreateCategory(name, type);

            // Assert
            Assert.Equal(category, result);
            _factoryMock.Verify(f => f.CreateCategory(name, type), Times.Once);
            _repositoryMock.Verify(r => r.Add(category), Times.Once);
        }

        [Fact]
        public void GetCategory_ShouldReturnCategory()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var category = Category.Create("Food", CategoryType.Expense);
            
            _repositoryMock.Setup(r => r.GetById(categoryId)).Returns(category);

            // Act
            var result = _facade.GetCategory(categoryId);

            // Assert
            Assert.Equal(category, result);
            _repositoryMock.Verify(r => r.GetById(categoryId), Times.Once);
        }

        [Fact]
        public void GetAllCategories_ShouldReturnAllCategories()
        {
            // Arrange
            var categories = new[]
            {
                Category.Create("Food", CategoryType.Expense),
                Category.Create("Salary", CategoryType.Income)
            };
            
            _repositoryMock.Setup(r => r.GetAll()).Returns(categories);

            // Act
            var result = _facade.GetAllCategories().ToList();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Contains(result, c => c.Name == "Food");
            Assert.Contains(result, c => c.Name == "Salary");
        }
    }
}