using CSESoftware.Core.Entity;
using CSESoftware.Repository.Builder;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

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
            where TEntity : class, IEntity
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

        protected IQueryable<TOut> GetQueryableSelect<TEntity, TOut>(IQueryWithSelect<TEntity, TOut> filter = null)
            where TEntity : class, IEntity
        {
            if (filter?.Select == null)
                throw new ArgumentException("Select not found");

            var query = GetQueryable(filter);
            return query.Select(filter.Select);
        }

        public virtual Task<List<TEntity>> GetAllAsync<TEntity>(IQuery<TEntity> filter)
            where TEntity : class, IEntity
        {
            return GetQueryable(filter).ToListAsync();
        }

        public virtual Task<List<TEntity>> GetAllAsync<TEntity>(Expression<Func<TEntity, bool>> filter = null)
            where TEntity : class, IEntity
        {
            return GetAllAsync(new QueryBuilder<TEntity>().Where(filter).Build());
        }

        public virtual Task<TEntity> GetFirstAsync<TEntity>(IQuery<TEntity> filter)
            where TEntity : class, IEntity
        {
            return GetQueryable(filter).FirstOrDefaultAsync();
        }

        public virtual Task<TEntity> GetFirstAsync<TEntity>(Expression<Func<TEntity, bool>> filter = null)
            where TEntity : class, IEntity
        {
            return GetFirstAsync(new QueryBuilder<TEntity>()
                .Where(filter)
                .Build());
        }

        public virtual Task<int> GetCountAsync<TEntity>(IQuery<TEntity> filter)
            where TEntity : class, IEntity
        {
            return GetQueryable(filter).CountAsync();
        }

        public virtual Task<int> GetCountAsync<TEntity>(Expression<Func<TEntity, bool>> filter = null)
            where TEntity : class, IEntity
        {
            return GetCountAsync(new QueryBuilder<TEntity>().Where(filter).Build());
        }

        public virtual Task<bool> GetExistsAsync<TEntity>(IQuery<TEntity> filter)
            where TEntity : class, IEntity
        {
            return GetQueryable(filter).AnyAsync();
        }

        public virtual Task<bool> GetExistsAsync<TEntity>(Expression<Func<TEntity, bool>> filter = null)
            where TEntity : class, IEntity
        {
            return GetQueryable(new QueryBuilder<TEntity>().Where(filter).Build()).AnyAsync();
        }

        public virtual Task<List<TOut>> GetAllWithSelectAsync<TEntity, TOut>(IQueryWithSelect<TEntity, TOut> filter = null) where TEntity : class, IEntity
        {
            return GetQueryableSelect(filter).ToListAsync();
        }

        public Task<TOut> GetFirstWithSelectAsync<TEntity, TOut>(IQueryWithSelect<TEntity, TOut> filter = null) where TEntity : class, IEntity
        {
            return GetQueryableSelect(filter).FirstOrDefaultAsync();
        }
    }
}
