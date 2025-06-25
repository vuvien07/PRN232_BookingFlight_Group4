using BookingFlightServer.Entities;
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

		public IQueryable<T> FindAll(bool trackChanges) =>
			!trackChanges ? _repositoryDbContext.Set<T>().AsNoTracking() : _repositoryDbContext.Set<T>();

		public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression, bool trackChanges)
		{
			var result = !trackChanges ? _repositoryDbContext.Set<T>().AsNoTracking() : _repositoryDbContext.Set<T>();
			return result.Where(expression);
		}

		public IQueryable<TResult> FindByCondition<TResult>(IQueryable<TResult> query, Expression<Func<TResult, bool>> expression)
		{
			return query.Where(expression);
		}

		public Task Update(T entity)
		{
			_repositoryDbContext.Set<T>().Update(entity);
			return _repositoryDbContext.SaveChangesAsync();
		}
	}
}
