using CSESoftware.Core.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CSESoftware.Repository.EntityFrameworkCore
{
    public class Repository<TContext> : ReadOnlyRepository<TContext>, IRepository
        where TContext : DbContext
    {
        public Repository(TContext context) : base(context)
        {
        }

        public virtual async Task CreateAsync<T>(T entity)
            where T : class
        {
            await Context.Set<T>().AddAsync(entity);
            await SaveAsync();
        }

        public virtual async Task CreateAsync<T>(List<T> entities)
            where T : class
        {
            await Context.Set<T>().AddRangeAsync(entities);
            await SaveAsync();
        }

        public virtual async Task UpdateAsync<T>(T entity)
            where T : class
        {
            if (Context.Entry(entity).State == EntityState.Detached)
                Context.Set<T>().Attach(entity);

            Context.Entry(entity).State = EntityState.Modified;
            await SaveAsync();
        }

        public virtual async Task UpdateAsync<T>(List<T> entities)
            where T : class
        {
            Context.Set<T>().AttachRange(
                entities.Where(x => Context.Entry(x).State == EntityState.Detached));

            foreach (var entity in entities)
            {
                Context.Entry(entity).State = EntityState.Modified;
            }
            await SaveAsync();
        }

        public virtual async Task DeleteAsync<T, TId>(TId id)
            where T : class, IEntityWithId<TId>
        {
            var entity = await Context.Set<T>().FindAsync(id);
            await DeleteAsync(entity);
        }

        public virtual async Task DeleteAsync<T>(T entity)
            where T : class
        {
            var dbSet = Context.Set<T>();
            if (Context.Entry(entity).State == EntityState.Detached)
                dbSet.Attach(entity);

            dbSet.Remove(entity);
            await SaveAsync();
        }

        public virtual async Task DeleteAsync<T>(List<T> entities)
            where T : class
        {
            Context.Set<T>().AttachRange(
                entities.Where(x => Context.Entry(x).State == EntityState.Detached));

            Context.Set<T>().RemoveRange(entities);
            await SaveAsync();
        }

        public virtual async Task DeleteAsync<T>(Expression<Func<T, bool>> filter)
            where T : class
        {
            Context.Set<T>().RemoveRange(Context.Set<T>().Where(filter));
            await SaveAsync();
        }

        private async Task SaveAsync()
        {
            await Context.SaveChangesAsync();
            DetachAllEntities();
        }

        private void DetachAllEntities()
        {
            Context.ChangeTracker.Entries().ToList()
                .ForEach(x => x.State = EntityState.Detached);
        }
    }
}
