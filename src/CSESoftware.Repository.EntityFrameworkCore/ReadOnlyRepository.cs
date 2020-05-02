using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CSESoftware.Core.Entity;
using CSESoftware.Repository.Builder;
using Microsoft.EntityFrameworkCore;

namespace CSESoftware.Repository.EntityFrameworkCore
{
    public class ReadOnlyRepository<TContext> : IReadOnlyRepository
        where TContext : DbContext
    {
        protected readonly TContext Context;

        public ReadOnlyRepository(TContext context)
        {
            Context = context;
        }

        protected IQueryable<TEntity> GetQueryable<TEntity>(IQuery<TEntity> filter)
                    where TEntity : class, IBaseEntity
        {
            if (filter == null) filter = new Query<TEntity>
            {
                Include = new List<Expression<Func<TEntity, object>>>()
            };

            IQueryable<TEntity> query = Context.Set<TEntity>();

            if (filter.Predicate != null)
                query = query.Where(filter.Predicate);

            query = filter.Include.Aggregate(query, (current, property) => current.Include(property));

            if (filter.OrderBy != null)
                query = filter.OrderBy(query);

            if (filter.Skip.HasValue)
                query = query.Skip(filter.Skip.Value);

            if (filter.Take.HasValue)
                query = query.Take(filter.Take.Value);

            return query;
        }

        protected IQueryable<object> GetQueryableSelect<TEntity>(IQuery<TEntity> filter = null)
            where TEntity : class, IBaseEntity
        {
            var query = GetQueryable(filter);

            if (filter?.Select != null)
                return query.Select(filter.Select);

            return query;
        }

        public virtual async Task<IEnumerable<TEntity>> GetAllAsync<TEntity>(IQuery<TEntity> filter = null)
            where TEntity : class, IBaseEntity
        {
            return await GetQueryable(filter).ToListAsync();
        }

        public virtual async Task<IEnumerable<TEntity>> GetAllAsync<TEntity>(Expression<Func<TEntity, bool>> filter)
            where TEntity : class, IBaseEntity
        {
            return await GetQueryable(new QueryBuilder<TEntity>().Where(filter).Build()).ToListAsync();
        }

        public virtual async Task<IEnumerable<object>> GetAllWithSelectAsync<TEntity>(IQuery<TEntity> filter = null)
            where TEntity : class, IBaseEntity
        {
            return await GetQueryableSelect(filter).ToListAsync();
        }

        public virtual async Task<TEntity> GetFirstAsync<TEntity>(IQuery<TEntity> filter = null)
            where TEntity : class, IBaseEntity
        {
            return await GetQueryable(new QueryBuilder<TEntity>()
                .Where(filter?.Predicate)
                .Include(filter?.Include)
                .OrderBy(filter?.OrderBy)
                .Build()).FirstOrDefaultAsync();
        }

        public virtual Task<int> GetCountAsync<TEntity>(Expression<Func<TEntity, bool>> filter = null)
            where TEntity : class, IBaseEntity
        {
            return GetQueryable(new QueryBuilder<TEntity>().Where(filter).Build()).CountAsync();
        }

        public virtual Task<bool> GetExistsAsync<TEntity>(Expression<Func<TEntity, bool>> filter = null)
            where TEntity : class, IBaseEntity
        {
            return GetQueryable(new QueryBuilder<TEntity>().Where(filter).Build()).AnyAsync();
        }
    }
}