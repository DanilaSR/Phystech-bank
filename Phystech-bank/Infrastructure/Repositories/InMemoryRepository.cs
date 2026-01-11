using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FinancialTracking.Application.Interfaces;

namespace FinancialTracking.Infrastructure.Repositories
{
    public class InMemoryRepository<T> : IRepository<T> where T : class
    {
        private readonly Dictionary<Guid, T> _storage = new();

        public void Add(T entity)
        {
            var id = GetEntityId(entity);
            _storage[id] = entity;
        }

        public void Update(T entity)
        {
            var id = GetEntityId(entity);
            _storage[id] = entity; // Overwrites if exists, adds if not
        }

        public void Delete(Guid id)
        {
            _storage.Remove(id);
        }

        public T? GetById(Guid id)
        {
            return _storage.TryGetValue(id, out var entity) ? entity : null;
        }

        public IEnumerable<T> GetAll()
        {
            return _storage.Values;
        }

        private Guid GetEntityId(T entity)
        {
            // Используем reflection для получения Id
            var idProperty = entity.GetType().GetProperty("Id");
            if (idProperty == null)
            {
                throw new InvalidOperationException($"Entity of type {typeof(T).Name} does not have an Id property");
            }
            
            var idValue = idProperty.GetValue(entity);
            if (idValue is Guid id)
            {
                return id;
            }
            
            throw new InvalidOperationException($"Id property of {typeof(T).Name} is not of type Guid");
        }
    }
}