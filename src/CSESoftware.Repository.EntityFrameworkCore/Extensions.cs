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
        internal static IQueryable<T> ApplyIncludes<T>(this IQueryable<T> query,
            List<Expression<Func<T, object>>> includes)
            where T : class
        {
            return includes.Aggregate(query, (current, property) => current.Include(property.ToPropertyString()));
        }

        internal static string ToPropertyString<T>(this Expression<Func<T, object>> property)
        {
            var modifiedProperty = property.ToString();

            modifiedProperty = Regex.Replace(modifiedProperty, @"^.*?\.", "");
            modifiedProperty = Regex.Replace(modifiedProperty, @"Select\(.*?\.", "");
            modifiedProperty = modifiedProperty.Replace(")", "");

            return modifiedProperty;
        }
    }
}
