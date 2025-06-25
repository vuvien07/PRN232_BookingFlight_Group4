using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace BookingFlightServer.Repositories
{
	public interface IBaseRepository<T>
	{
		IQueryable<T> FindAll(Func<IQueryable<T>, IIncludableQueryable<T, object>>? includes = null, bool trackChanges = false);
		IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression, Func<IQueryable<T>, IIncludableQueryable<T, object>>? includes = null, bool trackChanges = false);
		Task<T?> GetByCondition(Expression<Func<T, bool>> expression, Func<IQueryable<T>, IIncludableQueryable<T, object>>? includes = null, bool trackChanges = false);
		IQueryable<TResult> FindByCondition<TResult>(IQueryable<TResult> query, Expression<Func<TResult, bool>> expression);
		Task Create(T entity);
		Task Update(T entity);
		Task Delete(T entity);
	}
}
