namespace Ecommerce.Application.Stores;

public interface IProductService
{
    Task<bool> RelatedToStoreAsync(int productId, int storeId);
    Task<bool> DeleteProductStoreRelation(int productId);
}
