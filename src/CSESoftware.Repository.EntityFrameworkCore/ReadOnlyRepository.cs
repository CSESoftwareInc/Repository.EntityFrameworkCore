using CSESoftware.Core.Entity;
using CSESoftware.Repository.Builder;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
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

            query = query.ApplyIncludes(filter.Include);

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
            var select = query.Select(filter.Select);

            return filter.Distinct ? select.Distinct() : select;
        }

        public virtual Task<List<TEntity>> GetAllAsync<TEntity>(IQuery<TEntity> filter)
            where TEntity : class, IEntity
        {
            return GetQueryable(filter).ToListAsync(filter?.CancellationToken ?? CancellationToken.None);
        }

        public virtual List<TEntity> GetAll<TEntity>(IQuery<TEntity> filter)
            where TEntity : class, IEntity
        {
            return GetQueryable(filter).ToList();
        }

        public virtual Task<List<TEntity>> GetAllAsync<TEntity>(Expression<Func<TEntity, bool>> filter = null)
            where TEntity : class, IEntity
        {
            return GetAllAsync(new QueryBuilder<TEntity>().Where(filter).Build());
        }

        public virtual List<TEntity> GetAll<TEntity>(Expression<Func<TEntity, bool>> filter = null)
            where TEntity : class, IEntity
        {
            return GetAll(new QueryBuilder<TEntity>().Where(filter).Build());
        }

        public virtual Task<TEntity> GetFirstAsync<TEntity>(IQuery<TEntity> filter)
            where TEntity : class, IEntity
        {
            return GetQueryable(filter).FirstOrDefaultAsync(filter?.CancellationToken ?? CancellationToken.None);
        }

        public virtual TEntity GetFirst<TEntity>(IQuery<TEntity> filter)
            where TEntity : class, IEntity
        {
            return GetQueryable(filter).FirstOrDefault();
        }

        public virtual Task<TEntity> GetFirstAsync<TEntity>(Expression<Func<TEntity, bool>> filter = null)
            where TEntity : class, IEntity
        {
            return GetFirstAsync(new QueryBuilder<TEntity>()
                .Where(filter)
                .Build());
        }

        public virtual TEntity GetFirst<TEntity>(Expression<Func<TEntity, bool>> filter = null)
            where TEntity : class, IEntity
        {
            return GetFirst(new QueryBuilder<TEntity>()
                .Where(filter)
                .Build());
        }

        public virtual Task<int> GetCountAsync<TEntity>(IQuery<TEntity> filter)
            where TEntity : class, IEntity
        {
            return GetQueryable(filter).CountAsync(filter?.CancellationToken ?? CancellationToken.None);
        }

        public virtual Task<int> GetCountAsync<TEntity>(Expression<Func<TEntity, bool>> filter = null)
            where TEntity : class, IEntity
        {
            return GetCountAsync(new QueryBuilder<TEntity>().Where(filter).Build());
        }

        public virtual Task<bool> GetExistsAsync<TEntity>(IQuery<TEntity> filter)
            where TEntity : class, IEntity
        {
            return GetQueryable(filter).AnyAsync(filter?.CancellationToken ?? CancellationToken.None);
        }

        public virtual Task<bool> GetExistsAsync<TEntity>(Expression<Func<TEntity, bool>> filter = null)
            where TEntity : class, IEntity
        {
            return GetQueryable(new QueryBuilder<TEntity>().Where(filter).Build()).AnyAsync();
        }

        public virtual Task<List<TOut>> GetAllWithSelectAsync<TEntity, TOut>(IQueryWithSelect<TEntity, TOut> filter = null) where TEntity : class, IEntity
        {
            return GetQueryableSelect(filter).ToListAsync(filter?.CancellationToken ?? CancellationToken.None);
        }

        public virtual List<TOut> GetAllWithSelect<TEntity, TOut>(IQueryWithSelect<TEntity, TOut> filter = null) where TEntity : class, IEntity
        {
            return GetQueryableSelect(filter).ToList();
        }

        public Task<TOut> GetFirstWithSelectAsync<TEntity, TOut>(IQueryWithSelect<TEntity, TOut> filter = null) where TEntity : class, IEntity
        {
            return GetQueryableSelect(filter).FirstOrDefaultAsync(filter?.CancellationToken ?? CancellationToken.None);
        }

        public TOut GetFirstWithSelect<TEntity, TOut>(IQueryWithSelect<TEntity, TOut> filter = null) where TEntity : class, IEntity
        {
            return GetQueryableSelect(filter).FirstOrDefault();
        }

        public Task<int> GetCountWithSelectAsync<TEntity, TOut>(IQueryWithSelect<TEntity, TOut> filter = null) where TEntity : class, IEntity
        {
            return GetQueryableSelect(filter).CountAsync(filter?.CancellationToken ?? CancellationToken.None);
        }
    }
}
