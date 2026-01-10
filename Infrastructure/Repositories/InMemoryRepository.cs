using System;
using System.Collections.Generic;
using System.Linq;
using FinancialTracking.Application.Interfaces;

namespace FinancialTracking.Infrastructure.Repositories
{
    public class InMemoryRepository<T> : IRepository<T> where T : class
    {
        private readonly Dictionary<Guid, T> _storage = new();

        public void Add(T entity)
        {
            var id = (Guid)entity.GetType().GetProperty("Id")!.GetValue(entity)!;
            _storage[id] = entity;
        }

        public void Update(T entity)
        {
            var id = (Guid)entity.GetType().GetProperty("Id")!.GetValue(entity)!;
            if (_storage.ContainsKey(id))
                _storage[id] = entity;
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
    }
}