using System.Linq.Expressions;

namespace EdFi.Ods.AdminApi.Infrastructure.Extensions
{
    public static class QueryExtensions
    {
        /// <summary>
        /// Apply pagination based on the offset and limit
        /// </summary>
        /// <typeparam name="T">Type of entity</typeparam>
        /// <param name="source">IQueryable entity list to apply the pagination</param>
        /// <param name="offset"></param>
        /// <param name="limit"></param>
        /// <param name="settings">App Setting values</param>
        /// <returns>Paginated list</returns>
        public static IQueryable<T> Paginate<T>(this IQueryable<T> source, int? offset, int? limit)
        {
            try
            {
                if (offset != null)
                    source = source.Skip(offset.Value);

                if (limit != null)
                    source = source.Take(limit.Value);

                return source;
            }
            catch (Exception)
            {
                // If this throws an exception simply don't paginate.
                return source;
            }
        }
    }
}
