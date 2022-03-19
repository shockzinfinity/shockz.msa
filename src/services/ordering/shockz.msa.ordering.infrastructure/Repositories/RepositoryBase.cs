﻿using Microsoft.EntityFrameworkCore;
using shockz.msa.ordering.application.Contracts.Persistence;
using shockz.msa.ordering.domain.Common;
using shockz.msa.ordering.infrastructure.Persistence;
using System.Linq.Expressions;

namespace shockz.msa.ordering.infrastructure.Repositories
{
  public class RepositoryBase<T> : IAsyncRepository<T> where T : EntityBase
  {
    protected readonly OrderContext _context;

    public RepositoryBase(OrderContext context)
    {
      _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<IReadOnlyList<T>> GetAllAsync()
    {
      return await _context.Set<T>().ToListAsync();
    }

    public async Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>> predicate)
    {
      return await _context.Set<T>().Where(predicate).ToListAsync();
    }

    public async Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>> predicate = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string includeString = null, bool disableTracking = true)
    {
      IQueryable<T> query = _context.Set<T>();

      if (disableTracking) query = query.AsNoTracking();

      if (!string.IsNullOrWhiteSpace(includeString)) query = query.Include(includeString);

      if (predicate != null) query = query.Where(predicate);

      if (orderBy != null)
        return await orderBy(query).ToListAsync();

      return await query.ToListAsync();
    }

    public async Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>> predicate = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, List<Expression<Func<T, object>>> includes = null, bool disableTracking = true)
    {
      IQueryable<T> query = _context.Set<T>();

      if (disableTracking) query = query.AsNoTracking();

      if (includes != null) query = includes.Aggregate(query, (current, include) => current.Include(include));

      if (predicate != null) query = query.Where(predicate);

      if (orderBy != null)
        return await orderBy(query).ToListAsync();

      return await query.ToListAsync();
    }

    public virtual async Task<T> GetByIdAsync(int id)
    {
      return await _context.Set<T>().FindAsync(id);
    }

    public async Task<T> AddAsync(T entity)
    {
      _context.Set<T>().Add(entity);
      await _context.SaveChangesAsync();

      return entity;
    }

    public async Task UpdateAsync(T entity)
    {
      _context.Entry(entity).State = EntityState.Modified;
      await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(T entity)
    {
      _context.Set<T>().Remove(entity);
      await _context.SaveChangesAsync();
    }
  }
}
