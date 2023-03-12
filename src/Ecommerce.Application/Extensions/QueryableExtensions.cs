using Ecommerce.Application.Common.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Application.Extensions;

public static class QueryableExtensions
{
    public static IQueryable<TSource> Paginate<TSource>(this IQueryable<TSource> source, Pagination pagination)
    {
        return source
            .Skip((pagination.PageNumber - 1) * pagination.PageSize)
            .Take(pagination.PageSize);
    }

    public static Task<PaginatedList<TSource>> PaginatedListAsync<TSource>(this IQueryable<TSource> queryable, Pagination pagination) where TSource : class
        => PaginatedList<TSource>.CrateAsync(queryable.AsNoTracking(), pagination);
}