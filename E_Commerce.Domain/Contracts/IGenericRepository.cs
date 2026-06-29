using E_Commerce.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Domain.Contracts
{
    public interface IGenericRepository<TEntity , TKey> where TEntity : BaseEntity<TKey>
    {
        void Add(TEntity entity);
        void Update(TEntity entity);
        void Remove(TEntity entity);
        Task<TEntity?> GetByIdAsync(TKey id , CancellationToken ct = default);
        Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken ct = default);
    }
}
