namespace Ecommerce.Core.Interfaces;

public interface IProductService
{
    Task<int> RelatedToStoreAsync(int productId, int storeId);
    void DeleteProductStoreRelation(int productId);
}
