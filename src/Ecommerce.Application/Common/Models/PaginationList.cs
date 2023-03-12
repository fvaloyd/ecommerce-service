using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Application.Common.Models;

public class PaginatedList<T>
{
    public IReadOnlyCollection<T> Items {get;}
    public int TotalCount {get;}
    public int PageNumber {get;}
    public int TotalPages {get;}

    public PaginatedList(IReadOnlyCollection<T> items, int count, int pageSize, int pageNumber)
    {
        PageNumber = pageNumber;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        TotalCount = count;
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