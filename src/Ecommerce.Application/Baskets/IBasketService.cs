using Ecommerce.Core.Entities;
using Francisvac.Result;

namespace Ecommerce.Application.Baskets;

public interface IBasketService
{
    Task<Result> AddProductAsync(int productId, string userId);
    Task<Result> IncreaseProduct(int productId, string userId);
    Task<Result> DecreaseProduct(int productId, string userId);
    Task<Result> RestoreTheQuantityIntoStore(Basket basket);
    Task<Result<(IEnumerable<Product>, float)>> GetAllProducts(string userId);
    Task<Result> RemoveProduct(int productId, string UserId);
    Task<Result<int[]>> GetProductIds(string userId);
}
