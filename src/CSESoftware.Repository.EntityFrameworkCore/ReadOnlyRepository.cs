using CSESoftware.Repository.Query;
using CSESoftware.Repository.Query.Extensions;
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

        protected IQueryable<T> GetQueryable<T>(IQuery<T> filter)
            where T : class
        {
            if (filter == null) filter = new Query<T>
            {
                Include = new List<Expression<Func<T, object>>>()
            };

            IQueryable<T> query = Context.Set<T>();

            if (filter.Predicate != null)
                query = query.Where(filter.Predicate);

            query = query.ApplyIncludes(filter.Include);

            if (filter.Order != null)
                query = filter.Order(query);

            if (filter.Skip.HasValue)
                query = query.Skip(filter.Skip.Value);

            if (filter.Take.HasValue)
                query = query.Take(filter.Take.Value);

            return query;
        }

        protected IQueryable<TOut> GetQueryableSelect<T, TOut>(ISelectQuery<T, TOut> filter = null)
            where T : class
        {
            if (filter?.Select == null)
                throw new ArgumentException("Select not found");

            var query = GetQueryable(filter);
            return query.Select(filter.Select);
        }

        public virtual Task<List<T>> GetAllAsync<T>(IQuery<T> filter)
            where T : class
        {
            return GetQueryable(filter).ToListAsync();
        }

        public virtual Task<List<T>> GetAllAsync<T>(Expression<Func<T, bool>> filter = null)
            where T : class
        {
            return GetAllAsync(new Query<T>().Where(filter));
        }

        public virtual Task<T> GetFirstAsync<T>(IQuery<T> filter)
            where T : class
        {
            return GetQueryable(filter).FirstOrDefaultAsync();
        }

        public virtual Task<T> GetFirstAsync<T>(Expression<Func<T, bool>> filter = null)
            where T : class
        {
            return GetFirstAsync(new Query<T>()
                .Where(filter));
        }

        public virtual Task<int> GetCountAsync<T>(IQuery<T> filter)
            where T : class
        {
            return GetQueryable(filter).CountAsync();
        }

        public virtual Task<int> GetCountAsync<T>(Expression<Func<T, bool>> filter = null)
            where T : class
        {
            return GetCountAsync(new Query<T>().Where(filter));
        }

        public virtual Task<bool> GetExistsAsync<T>(IQuery<T> filter)
            where T : class
        {
            return GetQueryable(filter).AnyAsync();
        }

        public virtual Task<bool> GetExistsAsync<T>(Expression<Func<T, bool>> filter = null)
            where T : class
        {
            return GetQueryable(new Query<T>().Where(filter)).AnyAsync();
        }

        public virtual Task<List<TOut>> GetAllWithSelectAsync<T, TOut>(ISelectQuery<T, TOut> filter = null)
            where T : class
        {
            return GetQueryableSelect(filter).ToListAsync();
        }

        public Task<TOut> GetFirstWithSelectAsync<T, TOut>(ISelectQuery<T, TOut> filter = null)
            where T : class
        {
            return GetQueryableSelect(filter).FirstOrDefaultAsync();
        }
    }
}
