namespace Ecommerce.Application.Common.Models;

public class Pagination
{
    public Pagination(int pageSize, int pageNumber)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;
    }
    public int PageSize { get; set; }
    public int PageNumber { get; set; }
}