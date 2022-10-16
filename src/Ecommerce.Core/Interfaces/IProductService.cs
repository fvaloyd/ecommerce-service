namespace Ecommerce.Core.Interfaces;

public interface IProductService
{
    Task<int> RelatedToStoreAsync(int productId, int storeId);
    Task DeleteProductStoreRelation(int productId);
}
