namespace Ecommerce.Core.Interfaces;

public interface IProductService
{
    Task<bool> RelatedToStoreAsync(int productId, int storeId);
    void DeleteProductStoreRelation(int productId);
}
