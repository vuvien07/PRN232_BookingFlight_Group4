using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library
{
    public class RedisHelper
    {
        private readonly IDatabase _db;
        public RedisHelper(string connection)
        {
            var redis = ConnectionMultiplexer.Connect(connection);
            _db = redis.GetDatabase();
        }
		public async Task SetAsync(string key, string value, TimeSpan? expiry = null)
		{
			await _db.StringSetAsync(key, value, expiry);
		}

		public async Task<string?> GetAsync(string key)
		{
			return await _db.StringGetAsync(key);
		}

		public async Task<bool> DeleteAsync(string key)
		{
			return await _db.KeyDeleteAsync(key);
		}

		public async Task<bool> ExistsAsync(string key)
		{
			return await _db.KeyExistsAsync(key);
		}
	}
}
