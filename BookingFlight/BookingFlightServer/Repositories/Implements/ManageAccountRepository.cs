﻿using BookingFlightServer.Data;
using BookingFlightServer.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookingFlightServer.Repositories.Implements
{
    public class ManageAccountRepository : IManageAccountRepository
    {
        private readonly BookingFlightContext bookingFlightContext;

        public ManageAccountRepository(BookingFlightContext bookingFlightContext)
        {
            this.bookingFlightContext = bookingFlightContext;
        }

        public async Task<Account?> CreateAccountAsync(Account account)
        {
            bookingFlightContext.Accounts.Add(account);
            await bookingFlightContext.SaveChangesAsync();
            return account == null ? null : account;
        }

        public async Task<List<Account>?> GetAccountsAsync()
        {
            var accounts = await bookingFlightContext.Accounts.Include(a => a.Role)
                                                        .Include(a => a.Status)
                                                        .ToListAsync();
            return accounts == null ? null : accounts;
        }
    }
}
