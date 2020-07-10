using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CSESoftware.Core.Entity;
using Microsoft.EntityFrameworkCore;

namespace CSESoftware.Repository.EntityFrameworkCore
{
    public class Repository<TContext> : ReadOnlyRepository<TContext>, IRepository where TContext : DbContext
    {
        public Repository(TContext context) : base(context)
        {
        }

        public virtual void Create<TEntity>(TEntity entity)
            where TEntity : class, IEntity
        {
            if (entity is IActiveEntity activeEntity)
                entity = SetIsActiveToTrue<TEntity>(activeEntity);

            if (entity is IModifiedEntity modifiedEntity)
                entity = SetCreatedDateToNow<TEntity>(modifiedEntity);

            Context.Set<TEntity>().Add(entity);
        }

        public virtual void Create<TEntity>(List<TEntity> entities)
            where TEntity : class, IEntity
        {
            if (entities is List<IActiveEntity> activeEntities)
                entities = SetIsActiveToTrue<TEntity>(activeEntities);

            if (entities is List<IModifiedEntity> modifiedEntities)
                entities = SetCreatedDateToNow<TEntity>(modifiedEntities);

            Context.Set<TEntity>().AddRange(entities);
        }

        public virtual void Update<TEntity>(TEntity entity)
            where TEntity : class, IEntity
        {
            if (entity is IModifiedEntity modifiedEntity)
                entity = SetModifiedDateToNow<TEntity>(modifiedEntity);
            Context.Set<TEntity>().Attach(entity);
            Context.Entry(entity).State = EntityState.Modified;
        }

        public virtual void Update<TEntity>(List<TEntity> entities)
            where TEntity : class, IEntity
        {
            if (entities is List<IModifiedEntity> modifiedEntities)
                entities = SetModifiedDateToNow<TEntity>(modifiedEntities);

            Context.Set<TEntity>().AttachRange(entities);
            foreach (var entity in entities)
            {
                Context.Entry(entity).State = EntityState.Modified;
            }
        }

        public virtual void Delete<TEntity>(object id)
            where TEntity : class, IEntityWithId
        {
            var entity = Context.Set<TEntity>().Find(id);
            Delete(entity);
        }

        public virtual void Delete<TEntity>(TEntity entity)
            where TEntity : class, IEntity
        {
            var dbSet = Context.Set<TEntity>();
            if (Context.Entry(entity).State == EntityState.Detached)
            {
                dbSet.Attach(entity);
            }
            dbSet.Remove(entity);
        }

        public virtual void Delete<TEntity>(List<TEntity> entities)
            where TEntity : class, IEntity
        {
            Context.Set<TEntity>().AttachRange(
                entities.Where(x => Context.Entry(x).State == EntityState.Detached));

            Context.Set<TEntity>().RemoveRange(entities);
        }

        public virtual void Delete<TEntity>(Expression<Func<TEntity, bool>> filter)
            where TEntity : class, IEntity
        {
            Context.Set<TEntity>().RemoveRange(Context.Set<TEntity>().Where(filter));
        }

        public virtual Task SaveAsync()
        {
            return Context.SaveChangesAsync();
        }

        internal virtual TEntity SetIsActiveToTrue<TEntity>(IActiveEntity entity)
        {
            entity.IsActive = true;
            return (TEntity)entity;
        }

        internal virtual List<TEntity> SetIsActiveToTrue<TEntity>(List<IActiveEntity> entities)
        {
            return entities.Select(x =>
            {
                x.IsActive = true;
                return (TEntity)x;
            }).ToList();
        }

        internal virtual TEntity SetCreatedDateToNow<TEntity>(IModifiedEntity entity)
        {
            entity.CreatedDate = DateTime.UtcNow;
            return (TEntity)entity;
        }

        internal virtual List<TEntity> SetCreatedDateToNow<TEntity>(List<IModifiedEntity> entities)
        {
            return entities.Select(x =>
            {
                x.CreatedDate = DateTime.UtcNow;
                return (TEntity)x;
            }).ToList();
        }

        internal virtual TEntity SetModifiedDateToNow<TEntity>(IModifiedEntity entity)
        {
            entity.ModifiedDate = DateTime.UtcNow;
            return (TEntity)entity;
        }

        internal virtual List<TEntity> SetModifiedDateToNow<TEntity>(List<IModifiedEntity> entities)
        {
            return entities.Select(x =>
            {
                x.ModifiedDate = DateTime.UtcNow;
                return (TEntity)x;
            }).ToList();
        }
    }
}