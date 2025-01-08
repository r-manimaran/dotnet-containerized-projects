using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SharedLib.Repository;

public class Repository<T> : IRepository<T> where T : class
{
    private readonly DbContext _dbContext;
    private readonly DbSet<T> _dbSet;
    public Repository(DbContext dbContext)
    {
        _dbContext = dbContext;
        _dbSet = _dbContext.Set<T>();
    }
    public async Task AddAsync(T entity, CancellationToken cancellationToken=default)
    {
        await _dbSet.AddAsync(entity,cancellationToken);
    }

     public async Task UpdateAsync(T entity, CancellationToken cancellationToken=default)
    {
         _dbSet.Attach(entity);
         _dbContext.Entry(entity).State = EntityState.Modified;
    }
    public async Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
    {
       _dbSet.Remove(entity);
    }

     public async Task SaveChangesAsync(CancellationToken cancellationToken=default)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken=default)
    {
        return await _dbSet.ToListAsync(cancellationToken);
    }

    public async Task<T> GetByIdAsync(int id,CancellationToken cancellationToken=default)
    {
       return await _dbSet.FindAsync(id,cancellationToken);
    }

    public async Task<bool> Any(Expression<Func<T, bool>> expression, CancellationToken cancellationToken=default)
    {
       return await _dbSet.AnyAsync(expression,cancellationToken);
    }
   

    public Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_dbSet.Where(predicate).AsEnumerable());
    }

 
    public async Task<List<T>> GetAllFilter(Expression<Func<T, bool>> filter, CancellationToken cancellationToken = default)
    {
        return await _dbSet.Where(filter).ToListAsync(cancellationToken);
    }
       

    public async Task<T> GetFilter(Expression<Func<T, bool>> filter, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(filter,cancellationToken);
    }  

   
}
