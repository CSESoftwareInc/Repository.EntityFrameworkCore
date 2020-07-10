using System;
using CSESoftware.Core.Entity;

namespace CSESoftware.Repository.EntityFrameworkCore.Helpers
{
    internal static class PrepareService
    {
        internal static TEntity SetIsActiveToTrue<TEntity>(IActiveEntity entity)
        {
            entity.IsActive = true;
            return (TEntity)entity;
        }

        internal static TEntity SetCreatedDateToNow<TEntity>(IModifiedEntity entity)
        {
            entity.CreatedDate = DateTime.UtcNow;
            return (TEntity)entity;
        }

        internal static TEntity SetModifiedDateToNow<TEntity>(IModifiedEntity entity)
        {
            entity.ModifiedDate = DateTime.UtcNow;
            return (TEntity)entity;
        }
    }
}
