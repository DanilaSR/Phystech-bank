using System;
using System.Collections.Generic;

namespace FinancialTracking.Application.Interfaces
{
    public interface IRepository<T> where T : class
    {
        void Add(T entity);
        void Update(T entity);
        void Delete(Guid id);
        T? GetById(Guid id);
        IEnumerable<T> GetAll();
    }
}