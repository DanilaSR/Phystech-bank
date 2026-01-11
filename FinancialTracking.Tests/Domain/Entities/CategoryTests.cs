using Xunit;
using FinancialTracking.Domain.Entities;
using System;

namespace FinancialTracking.Tests.Domain.Entities
{
    public class CategoryTests
    {
        [Fact]
        public void CreateCategory_ValidData_ShouldCreateSuccessfully()
        {
            // Arrange
            var name = "Food";
            var type = CategoryType.Expense;

            // Act
            var category = Category.Create(name, type);

            // Assert
            Assert.NotNull(category);
            Assert.Equal(name, category.Name);
            Assert.Equal(type, category.Type);
            Assert.NotEqual(Guid.Empty, category.Id);
        }

        [Fact]
        public void CreateCategory_EmptyName_ShouldThrowArgumentException()
        {
            // Arrange
            var name = "";
            var type = CategoryType.Income;

            // Act & Assert
            Assert.Throws<ArgumentException>(() => Category.Create(name, type));
        }

        [Fact]
        public void CreateCategory_WithSpacesInName_ShouldCreateSuccessfully()
        {
            // Arrange
            var name = "  Food  ";
            var type = CategoryType.Expense;

            // Act
            var category = Category.Create(name, type);

            // Assert
            Assert.Equal(name, category.Name);
        }
    }
}