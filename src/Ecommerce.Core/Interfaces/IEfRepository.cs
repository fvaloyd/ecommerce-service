using System.Linq.Expressions;
using Ecommerce.Core.Entities;

namespace Ecommerce.Core.Interfaces;

public interface IEfRepository<T> where T : BaseEntity
{
    IEnumerable<T> GetAll(
        Expression<Func<T, bool>> Filter = null!,
        string IncludeProperty = null!);

    T GetFirst(
        Expression<Func<T, bool>> Filter = null!,
        string IncludeProperty = null!);

    Task<T> AddAsync(T entity);
    Task<bool> AddRangeAsync(IEnumerable<T> entities);

    void Update(T entity);

    Task RemoveAsync(int id);

    void Remove(T entity);

    void RemoveRange(IEnumerable<T> entities);

    Task<int> SaveChangeAsync();
}
