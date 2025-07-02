namespace BookingFlightServer.UnitOfWork
{
	public interface ITransactionDbManager
	{
		Task BeginTransactionAsync();
		Task CommitTransactionAsync();
		Task RollbackTransactionAsync();
	}
}
