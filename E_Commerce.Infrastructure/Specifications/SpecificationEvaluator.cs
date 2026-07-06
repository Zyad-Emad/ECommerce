using E_Commerce.Domain.Common;
using E_Commerce.Domain.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Infrastructure.Specifications
{
    internal static class SpecificationEvaluator
    {
        public static  IQueryable<TEntity> CreateQuery<TEntity , TKey>(IQueryable<TEntity> inputQuery , ISpecifications<TEntity , TKey> spec) where TEntity : BaseEntity<TKey>
        {
            var query = inputQuery;

            if (spec.Criteria is not null)
                query = query.Where(spec.Criteria);

            if (spec.IncludeExpressions.Any())
                query = spec.IncludeExpressions.Aggregate(query, (current, nextExp) => current.Include(nextExp));

            if (spec.OrderBy is not null)
                query = query.OrderBy(spec.OrderBy);
            else if (spec.OrderByDescending is not null)
                query = query.OrderByDescending(spec.OrderByDescending);

            if (spec.IsPaginated)
                query = query.Skip(spec.Skip).Take(spec.Take);


                return query;
        }
    }
}
