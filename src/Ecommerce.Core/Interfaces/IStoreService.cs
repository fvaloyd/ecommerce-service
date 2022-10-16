namespace Ecommerce.Core.Interfaces;

public interface IStoreService
{
    Task<bool> AddProductAsync(int productId, int storeId);
    Task<bool> IncreaseProductAsync(int productId, int storeId);
    Task<bool> DecreaseProductAsync(int productId, int storeId);
    Task DeleteRelationProduct(int storeId);
}
