using E_Commerce.Domain.Common;
using E_Commerce.Domain.Contracts;
using E_Commerce.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Infrastructure.Repositories
{
    internal class UnitOfWork(StoreDbContext dbContext) : IUnitOfWork
    {
        private readonly Dictionary<string, object> repositories = [];
        public IGenericRepository<TEntity, TKey> GetRepository<TEntity, TKey>() where TEntity : BaseEntity<TKey>
        {
            var typeName = typeof(TEntity).Name; 
            if(repositories.TryGetValue(typeName , out object? value))
                return (IGenericRepository<TEntity, TKey>) value;
            var repo = new GenericRepository<TEntity, TKey>(dbContext);
            repositories[typeName] = repo;
            return repo;
        }

        public async Task<int> SaveChangesAsync(CancellationToken ct = default)
            => await dbContext.SaveChangesAsync(ct);
    }
}
