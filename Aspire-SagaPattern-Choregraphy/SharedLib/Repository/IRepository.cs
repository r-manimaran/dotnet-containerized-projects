using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SharedLib.Repository;

public interface IRepository<T> where T : class
{
    Task<T> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> FindAllAsync(Expression<Func<T,bool>> predicate);
    Task<bool> Any(Expression<Func<T,bool>> expression);
    Task<T> GetFilter(Expression<Func<T,bool>> filter);
    Task<List<T>> GetAllFilter(Expression<Func<T,bool>> filter);

    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);

    Task SaveChangesAsync();
}
