using Francisvac.Result;

namespace Ecommerce.Application.Stores;

public interface IStoreService
{
    Task<Result> AddProductAsync(int productId, int storeId);
    Task<Result> IncreaseProductAsync(int productId, int storeId);
    Task<Result> DecreaseProductAsync(int productId, int storeId);
    Task<Result> DeleteProductStoreRelation(int storeId);
    Task<Result> DeleteStore(int storeId);
}
