namespace Ecommerce.Application.Stores;

public interface IStoreService
{
    Task<bool> AddProductAsync(int productId, int storeId);
    Task<bool> IncreaseProductAsync(int productId, int storeId);
    Task<bool> DecreaseProductAsync(int productId, int storeId);
    void DeleteProductStoreRelation(int storeId);
}
