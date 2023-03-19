using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Application.Common.Models;

public class PaginatedList<T>
{
    public IReadOnlyCollection<T> Items {get;}
    public int TotalCount {get;}
    public int PageNumber {get;}
    public int TotalPages {get;}

    private PaginatedList(IReadOnlyCollection<T> items, int count, int pageSize, int pageNumber)
    {
        PageNumber = pageNumber;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        TotalCount = count;
        Items = items;
    }

    public PaginatedList(int pageNumber, int totalPages, int totalCount, IReadOnlyCollection<T> items)
    {
        PageNumber = pageNumber;
        TotalPages = totalPages;
        TotalCount = totalCount;
        Items = items;
    }

    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;

    public static async Task<PaginatedList<T>> CrateAsync(IQueryable<T> source, Pagination pagination)
    {
        var count = await source.CountAsync();
        var items = await source.Skip((pagination.PageNumber - 1) * pagination.PageSize).Take(pagination.PageSize).ToListAsync();

        return new PaginatedList<T>(items, count, pagination.PageSize, pagination.PageNumber);
    }
}

public static class PaginatedListExtension
{
    public static PaginatedList<TDestination> To<TDestination, TSource>(
        this PaginatedList<TSource> source,
        IReadOnlyCollection<TDestination> items)
        => new PaginatedList<TDestination>(
            pageNumber: source.PageNumber,
            totalPages: source.TotalPages,
            totalCount: source.TotalCount,
            items: items);
}