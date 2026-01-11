using Xunit;
using FinancialTracking.Infrastructure.Repositories;
using FinancialTracking.Domain.Entities;
using System;
using System.Linq;

namespace FinancialTracking.Tests.Infrastructure.Repositories
{
    public class InMemoryRepositoryTests
    {
        private readonly InMemoryRepository<BankAccount> _repository;

        public InMemoryRepositoryTests()
        {
            _repository = new InMemoryRepository<BankAccount>();
        }

        [Fact]
        public void Add_ShouldStoreEntity()
        {
            // Arrange
            var account = BankAccount.Create("Test", 1000m);

            // Act
            _repository.Add(account);
            var retrieved = _repository.GetById(account.Id);

            // Assert
            Assert.NotNull(retrieved);
            Assert.Equal(account.Id, retrieved.Id);
            Assert.Equal(account.Name, retrieved.Name);
        }

        [Fact]
        public void GetById_NonExistentId_ShouldReturnNull()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();

            // Act
            var result = _repository.GetById(nonExistentId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void Update_ShouldModifyEntity()
        {
            // Arrange
            var account = BankAccount.Create("Test", 1000m);
            _repository.Add(account);
            
            // Create new account with same ID but different data
            var updatedAccount = BankAccount.Create("Updated", 2000m);
            // Note: In real scenario, we would have a method to update existing entity
            
            // Act
            _repository.Update(updatedAccount);
            var retrieved = _repository.GetById(account.Id);

            // Assert
            Assert.NotNull(retrieved);
            Assert.Equal("Updated", retrieved.Name);
        }

        [Fact]
        public void Delete_ShouldRemoveEntity()
        {
            // Arrange
            var account = BankAccount.Create("Test", 1000m);
            _repository.Add(account);

            // Act
            _repository.Delete(account.Id);
            var retrieved = _repository.GetById(account.Id);

            // Assert
            Assert.Null(retrieved);
        }

        [Fact]
        public void GetAll_ShouldReturnAllEntities()
        {
            // Arrange
            var account1 = BankAccount.Create("Test1", 1000m);
            var account2 = BankAccount.Create("Test2", 2000m);
            
            _repository.Add(account1);
            _repository.Add(account2);

            // Act
            var allAccounts = _repository.GetAll().ToList();

            // Assert
            Assert.Equal(2, allAccounts.Count);
            Assert.Contains(allAccounts, a => a.Id == account1.Id);
            Assert.Contains(allAccounts, a => a.Id == account2.Id);
        }
    }
}