using Ecommerce.Core.Entities;

namespace Ecommerce.Application.Baskets;

public interface IBasketService
{
    Task<bool> AddProductAsync(int productId, string userId);
    Task<bool> IncreaseProduct(int productId, string userId);
    Task<bool> DecreaseProduct(int productId, string userId);
    Task<bool> RestoreTheQuantityIntoStore(Basket basket);
    Task<IEnumerable<Product>> GetAllProducts(string userId);
}
