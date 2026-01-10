using System;
using System.Collections.Generic;
using System.Linq;
using FinancialTracking.Application.Interfaces;
using FinancialTracking.Domain.Entities;
using FinancialTracking.Domain.Factories;

namespace FinancialTracking.Application.Facades
{
    public interface ICategoryFacade
    {
        Category CreateCategory(string name, CategoryType type);
        void UpdateCategory(Guid id, string name);
        void DeleteCategory(Guid id);
        Category? GetCategory(Guid id);
        IEnumerable<Category> GetAllCategories();
        IEnumerable<Category> GetCategoriesByType(CategoryType type);
    }

    public class CategoryFacade : ICategoryFacade
    {
        private readonly IRepository<Category> _repository;
        private readonly IDomainFactory _factory;

        public CategoryFacade(IRepository<Category> repository, IDomainFactory factory)
        {
            _repository = repository;
            _factory = factory;
        }

        public Category CreateCategory(string name, CategoryType type)
        {
            var category = _factory.CreateCategory(name, type);
            _repository.Add(category);
            return category;
        }

        public void UpdateCategory(Guid id, string name)
        {
            var category = _repository.GetById(id);
            if (category != null)
            {
                // In real app, we would have Update method
                var updated = Category.Create(name, category.Type);
                _repository.Update(updated);
            }
        }

        public void DeleteCategory(Guid id)
        {
            _repository.Delete(id);
        }

        public Category? GetCategory(Guid id)
        {
            return _repository.GetById(id);
        }

        public IEnumerable<Category> GetAllCategories()
        {
            return _repository.GetAll();
        }

        public IEnumerable<Category> GetCategoriesByType(CategoryType type)
        {
            return _repository.GetAll().Where(c => c.Type == type);
        }
    }
}