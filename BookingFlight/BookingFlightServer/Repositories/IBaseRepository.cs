using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace BookingFlightServer.Repositories
{
	public interface IBaseRepository<T>
	{
		IQueryable<T> FindAll(bool trackChanges);
		IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression, bool trackChanges);
		IQueryable<TResult> FindByCondition<TResult>(IQueryable<TResult> query, Expression<Func<TResult, bool>> expression);
		Task Create(T entity);
		Task Update(T entity);
		Task Delete(T entity);
	}
}
