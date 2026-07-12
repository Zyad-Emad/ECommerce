using E_Commerce.Domain.Contracts;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Infrastructure.Repositories
{
    internal class CacheRepository : ICacheRepository
    {
        private readonly IDatabase database;
        public CacheRepository(IConnectionMultiplexer connection)
        {
            database = connection.GetDatabase();
        }
        public async Task<string?> GetAsync(string cacheKey, CancellationToken ct = default)
        {
            var value = await database.StringGetAsync(cacheKey);
            if (value.IsNullOrEmpty) return null;
            return value.ToString();
        }

        public async Task SetAsync(string cacheKey, string CacheValue, TimeSpan? timeToLive = null, CancellationToken ct = default)
        {
            await database.StringSetAsync(cacheKey, CacheValue, timeToLive ?? TimeSpan.FromDays(2));
        }
    }
}
