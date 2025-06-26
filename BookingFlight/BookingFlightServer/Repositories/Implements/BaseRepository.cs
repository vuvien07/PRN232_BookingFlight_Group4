using BookingFlightServer.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace BookingFlightServer.Repositories.Implements
{
	public abstract class BaseRepository<T> : IBaseRepository<T> where T : class
	{
		protected BookingFlightContext _repositoryDbContext;
		protected BaseRepository(BookingFlightContext repositoryDbContext)
		{
			_repositoryDbContext = repositoryDbContext;
		}

		public async Task Create(T entity)
		{
			_repositoryDbContext.Set<T>().Add(entity);
			await _repositoryDbContext.SaveChangesAsync();
		}

		public async Task Delete(T entity)
		{
			_repositoryDbContext.Set<T>().Remove(entity);
			await _repositoryDbContext.SaveChangesAsync();
		}

		public IQueryable<T> FindAll(Func<IQueryable<T>, IIncludableQueryable<T, object>>? includes = null, bool trackChanges = false)
		{
			IQueryable<T> query = _repositoryDbContext.Set<T>();
			if (includes != null)
			{
				query = includes(query);
			}
			if (!trackChanges)
			{
				query = query.AsNoTracking();
			}
			return query;
		}

		public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression, Func<IQueryable<T>, IIncludableQueryable<T, object>>? includes = null, bool trackChanges = false)
		{
			IQueryable<T> query = _repositoryDbContext.Set<T>();
			if (includes != null)
			{
				query = includes(query);
			}
			if (!trackChanges)
			{
				query = query.AsNoTracking();
			}
			return query.Where(expression);
		}

		public IQueryable<TResult> FindByCondition<TResult>(IQueryable<TResult> query, Expression<Func<TResult, bool>> expression)
		{
			return query.Where(expression);
		}

		public Task<T?> GetByCondition(Expression<Func<T, bool>> expression, Func<IQueryable<T>, IIncludableQueryable<T, object>>? includes = null, bool trackChanges = false)
		{
			IQueryable<T> query = _repositoryDbContext.Set<T>();
			if (includes != null)
			{
				query = includes(query);
			}
			if (!trackChanges)
			{
				query = query.AsNoTracking();
			}
			return query.FirstOrDefaultAsync(expression);
		}

		public async Task Update(T entity)
		{
			_repositoryDbContext.Set<T>().Update(entity);
			await _repositoryDbContext.SaveChangesAsync();
		}

	}
}
