using Xunit;
using FinancialTracking.Infrastructure.Repositories;
using FinancialTracking.Domain.Entities;
using System;
using System.Linq;

namespace FinancialTracking.Tests.Infrastructure.Repositories
{
    public class InMemoryRepositoryTests
    {
        [Fact]
        public void Add_ShouldStoreEntity()
        {
            // Arrange
            var repository = new InMemoryRepository<BankAccount>();
            var account = BankAccount.Create("Test", 1000m);

            // Act
            repository.Add(account);
            var retrieved = repository.GetById(account.Id);

            // Assert
            Assert.NotNull(retrieved);
            Assert.Equal(account.Id, retrieved.Id);
            Assert.Equal(account.Name, retrieved.Name);
            Assert.Equal(account.Balance, retrieved.Balance);
        }

        [Fact]
        public void GetById_NonExistentId_ShouldReturnNull()
        {
            // Arrange
            var repository = new InMemoryRepository<BankAccount>();

            // Act
            var result = repository.GetById(Guid.NewGuid());

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void Update_ForBankAccount_ShouldAddNewEntity()
        {
            // Arrange
            var repository = new InMemoryRepository<BankAccount>();
            var originalAccount = BankAccount.Create("Original", 1000m);
            repository.Add(originalAccount);

            // BankAccount is immutable - creates new ID
            var newAccount = BankAccount.Create("New", 2000m);

            // Act
            repository.Update(newAccount);
            
            // Both should exist (different IDs)
            var retrievedOriginal = repository.GetById(originalAccount.Id);
            var retrievedNew = repository.GetById(newAccount.Id);

            // Assert
            Assert.NotNull(retrievedOriginal); // Original still exists
            Assert.NotNull(retrievedNew); // New also exists
            Assert.Equal("Original", retrievedOriginal.Name);
            Assert.Equal("New", retrievedNew.Name);
        }

        [Fact]
        public void Update_WithTestEntity_ShouldReplaceWhenSameId()
        {
            // Arrange - using test entity that allows setting ID
            var repository = new InMemoryRepository<TestEntity>();
            var entityId = Guid.NewGuid();
            var original = new TestEntity { Id = entityId, Name = "Original" };
            repository.Add(original);
            
            var updated = new TestEntity { Id = entityId, Name = "Updated" };
            
            // Act
            repository.Update(updated);
            var retrieved = repository.GetById(entityId);
            
            // Assert
            Assert.NotNull(retrieved);
            Assert.Equal("Updated", retrieved.Name);
        }

        [Fact]
        public void Delete_ExistingEntity_ShouldRemove()
        {
            // Arrange
            var repository = new InMemoryRepository<BankAccount>();
            var account = BankAccount.Create("Test", 1000m);
            repository.Add(account);

            // Act
            repository.Delete(account.Id);
            var retrieved = repository.GetById(account.Id);

            // Assert
            Assert.Null(retrieved);
        }

        [Fact]
        public void Delete_NonExistentId_ShouldNotThrow()
        {
            // Arrange
            var repository = new InMemoryRepository<BankAccount>();

            // Act & Assert - should not throw
            var exception = Record.Exception(() => repository.Delete(Guid.NewGuid()));
            Assert.Null(exception);
        }

        [Fact]
        public void GetAll_EmptyRepository_ShouldReturnEmpty()
        {
            // Arrange
            var repository = new InMemoryRepository<BankAccount>();

            // Act
            var all = repository.GetAll().ToList();

            // Assert
            Assert.Empty(all);
        }

        [Fact]
        public void GetAll_MultipleEntities_ShouldReturnAll()
        {
            // Arrange
            var repository = new InMemoryRepository<BankAccount>();
            var account1 = BankAccount.Create("Test1", 1000m);
            var account2 = BankAccount.Create("Test2", 2000m);
            
            repository.Add(account1);
            repository.Add(account2);

            // Act
            var all = repository.GetAll().ToList();

            // Assert
            Assert.Equal(2, all.Count);
            Assert.Contains(all, a => a.Id == account1.Id);
            Assert.Contains(all, a => a.Id == account2.Id);
        }

        [Fact]
        public void Add_DuplicateId_ShouldOverwrite()
        {
            // Arrange - using test entity
            var repository = new InMemoryRepository<TestEntity>();
            var entityId = Guid.NewGuid();
            var entity1 = new TestEntity { Id = entityId, Name = "First" };
            var entity2 = new TestEntity { Id = entityId, Name = "Second" };
            
            // Act
            repository.Add(entity1);
            repository.Add(entity2); // Same ID, should overwrite
            
            var retrieved = repository.GetById(entityId);
            var count = repository.GetAll().Count();
            
            // Assert
            Assert.NotNull(retrieved);
            Assert.Equal("Second", retrieved.Name);
            Assert.Equal(1, count);
        }

        // Test helper class with mutable properties
        private class TestEntity
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
        }
    }
}