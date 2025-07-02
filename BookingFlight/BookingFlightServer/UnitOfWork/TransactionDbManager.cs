﻿using BookingFlightServer.Data;
using Microsoft.EntityFrameworkCore.Storage;

namespace BookingFlightServer.UnitOfWork
{
	public class TransactionDbManager : ITransactionDbManager
	{
		private readonly BookingFlightContext _context;
		private IDbContextTransaction? _transaction;

		public TransactionDbManager(BookingFlightContext context)
		{
			_context = context;
		}

		public async Task BeginTransactionAsync()
		{
			if (_transaction == null)
			{
				_transaction = await _context.Database.BeginTransactionAsync();
			}
		}

		public async Task CommitTransactionAsync()
		{
			if (_transaction != null)
			{
				await _context.SaveChangesAsync();
				await _transaction.CommitAsync();
				await _transaction.DisposeAsync();
				_transaction = null;
			}
		}

		public async Task RollbackTransactionAsync()
		{
			if (_transaction != null)
			{
				await _transaction.RollbackAsync();
				await _transaction.DisposeAsync();
				_transaction = null;
			}
		}
	}
}
