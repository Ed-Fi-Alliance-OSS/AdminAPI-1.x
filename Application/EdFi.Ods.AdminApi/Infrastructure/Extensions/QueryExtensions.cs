// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Linq.Expressions;
using System.Reflection;
using EdFi.Ods.AdminApi.Helpers;
using Microsoft.Extensions.Options;

namespace EdFi.Ods.AdminApi.Infrastructure.Extensions
{
    public static class QueryExtensions
    {
        /// <summary>
        /// Custom function for sorting.
        /// We initially try to fix by orderBy. If this column does not exist, we try to sort by orderByDefault.
        /// </summary>
        /// <typeparam name="T">Any entity from the model</typeparam>
        /// <param name="source"></param>
        /// <param name="orderBy">Try to fix by this column</param>
        /// <param name="orderByDefault">In case orderBy column does not exist, we sort by this column</param>
        /// <param name="descending">asc or desc</param>
        /// <returns></returns>
        public static IQueryable<T> OrderByColumn<T>(this IQueryable<T> source, string orderBy, string orderByDefault, bool descending = false)
        {
            try
            {
                var type = typeof(T);
                var property = type.GetProperty(orderBy, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                if (property == null)
                    property = type.GetProperty(orderByDefault, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                if (property != null)
                {
                    var parameter = Expression.Parameter(type, "p");
                    var propertyAccess = Expression.MakeMemberAccess(parameter, property);
                    var orderByExp = Expression.Lambda(propertyAccess, parameter);

                    var resultExp = Expression.Call(
                        typeof(Queryable),
                        descending ? "OrderByDescending" : "OrderBy",
                        new Type[] { type, property.PropertyType },
                        source.Expression,
                        Expression.Quote(orderByExp)
                    );

                    return source.Provider.CreateQuery<T>(resultExp);
                }
                return source;
            }
            catch (Exception)
            {
                /// If this throws an exception simply don't sort.
                return source;
            }
        }

        /// <summary>
        /// Apply pagination based on the offset and limit
        /// </summary>
        /// <typeparam name="T">Type of entity</typeparam>
        /// <param name="source">IQueryable entity list to apply the pagination</param>
        /// <param name="offset"></param>
        /// <param name="limit"></param>
        /// <param name="settings">App Setting values</param>
        /// <returns>Paginated list</returns>
        public static IQueryable<T> Paginate<T>(this IQueryable<T> source, int? offset, int? limit, IOptions<AppSettings> settings)
        {
            try
            {
                if (offset == null)
                    offset = settings.Value.DefaultPageSizeOffset;

                if (limit == null)
                    limit = settings.Value.DefaultPageSizeLimit;

                return source.Skip(offset.Value).Take(limit.Value);
            }
            catch (Exception)
            {
                // If this throws an exception simply don't paginate.
                return source;
            }
        }
    }
}
