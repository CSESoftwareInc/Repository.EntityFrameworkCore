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

        public virtual void Create<T>(T entity)
            where T : class
        {
            Context.Set<T>().Add(entity);
        }

        public virtual void Create<T>(List<T> entities)
            where T : class
        {
            Context.Set<T>().AddRange(entities);
        }

        public virtual void Update<T>(T entity)
            where T : class
        {
            if (Context.Entry(entity).State == EntityState.Detached)
                Context.Set<T>().Attach(entity);

            Context.Entry(entity).State = EntityState.Modified;
        }

        public virtual void Update<T>(List<T> entities)
            where T : class
        {
            Context.Set<T>().AttachRange(
                entities.Where(x => Context.Entry(x).State == EntityState.Detached));

            foreach (var entity in entities)
            {
                Context.Entry(entity).State = EntityState.Modified;
            }
        }

        public virtual void Delete<T>(T entity)
            where T : class
        {
            var dbSet = Context.Set<T>();
            if (Context.Entry(entity).State == EntityState.Detached)
                dbSet.Attach(entity);

            dbSet.Remove(entity);
        }

        public virtual void Delete<T>(List<T> entities)
            where T : class
        {
            Context.Set<T>().AttachRange(
                entities.Where(x => Context.Entry(x).State == EntityState.Detached));

            Context.Set<T>().RemoveRange(entities);
        }

        public virtual void Delete<T>(Expression<Func<T, bool>> filter)
            where T : class
        {
            Context.Set<T>().RemoveRange(Context.Set<T>().Where(filter));
        }

        public async Task SaveAsync()
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
