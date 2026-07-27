using E_Commerce.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Domain.Contracts
{
    public interface ISpecifications<TEntity , TKey> where TEntity : BaseEntity<TKey>
    {
        ICollection<Expression<Func<TEntity , object>>> IncludeExpressions { get; }
        Expression<Func<TEntity , bool>> Criteria { get; }
        Expression<Func<TEntity , object>>? OrderBy { get; }
        Expression<Func<TEntity , object>>? OrderByDescending { get; }
        int Take { get;}
        int Skip { get; }
        bool IsPaginated { get; }
    }
}
