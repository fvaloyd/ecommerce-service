namespace Ecommerce.Application.Stores;

public interface IProductService
{
    Task<bool> RelatedToStoreAsync(int productId, int storeId);
    void DeleteProductStoreRelation(int productId);
}
