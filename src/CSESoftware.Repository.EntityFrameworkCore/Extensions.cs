using CSESoftware.Core.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace CSESoftware.Repository.EntityFrameworkCore
{
    internal static class Extensions
    {
        internal static IQueryable<TEntity> ApplyIncludes<TEntity>(this IQueryable<TEntity> query,
            List<Expression<Func<TEntity, object>>> includes)
            where TEntity : class, IEntity
        {
            return includes.Aggregate(query, (current, property) => current.Include(property.ToPropertyString()));
        }

        internal static string ToPropertyString<TEntity>(this Expression<Func<TEntity, object>> property)
        {
            var modifiedProperty = property.ToString();

            modifiedProperty = Regex.Replace(modifiedProperty, @"^.*?\.", "");
            modifiedProperty = Regex.Replace(modifiedProperty, @"Select\(.*?\.", "");
            modifiedProperty = modifiedProperty.Replace(")", "");

            return modifiedProperty;
        }
    }
}
