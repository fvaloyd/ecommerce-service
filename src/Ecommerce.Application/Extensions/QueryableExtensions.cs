using Ecommerce.Application.Common.Models;

namespace Ecommerce.Application.Extensions;

public static class QueryableExtensions
{
    public static IQueryable<TSource> Paginate<TSource>(this IQueryable<TSource> source, Pagination pagination)
    {
        return source
            .Skip((pagination.PageNumber - 1) * pagination.PageSize)
            .Take(pagination.PageSize);
    }
}