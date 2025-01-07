using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SharedLib.Repository;

public class Repository<T> : IRepository<T> where T : class
{
    public Task AddAsync(T entity)
    {
        throw new NotImplementedException();
    }

    public Task<bool> Any(Expression<Func<T, bool>> expression)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(T entity)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>> predicate)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<T>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<List<T>> GetAllFilter(Expression<Func<T, bool>> filter)
    {
        throw new NotImplementedException();
    }

    public Task<T> GetByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<T> GetFilter(Expression<Func<T, bool>> filter)
    {
        throw new NotImplementedException();
    }

    public Task SaveChangesAsync()
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(T entity)
    {
        throw new NotImplementedException();
    }
}
