using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SharedLib.Repository;

public interface IRepository<T> where T : class
{
    Task<T> GetByIdAsync(int id, CancellationToken cancellationToken=default);
    Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken=default);
    Task<IEnumerable<T>> FindAllAsync(Expression<Func<T,bool>> predicate, CancellationToken cancellationToken=default);
    Task<bool> Any(Expression<Func<T,bool>> expression, CancellationToken cancellationToken=default);
    Task<T> GetFilter(Expression<Func<T,bool>> filter, CancellationToken cancellationToken=default);
    Task<List<T>> GetAllFilter(Expression<Func<T,bool>> filter, CancellationToken cancellationToken=default);

    Task AddAsync(T entity, CancellationToken cancellationToken=default);
    Task UpdateAsync(T entity, CancellationToken cancellationToken=default);
    Task DeleteAsync(T entity, CancellationToken cancellationToken=default);

    Task SaveChangesAsync(CancellationToken cancellationToken=default);
}
