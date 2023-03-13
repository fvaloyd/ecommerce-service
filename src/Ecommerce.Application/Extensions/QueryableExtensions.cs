using Ecommerce.Application.Common.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Application.Extensions;

public static class QueryableExtensions
{
    public static Task<PaginatedList<TSource>> PaginatedListAsync<TSource>(this IQueryable<TSource> queryable, Pagination pagination) where TSource : class
        => PaginatedList<TSource>.CrateAsync(queryable.AsNoTracking(), pagination);
}