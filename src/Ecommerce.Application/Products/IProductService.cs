namespace Ecommerce.Application.Products;

public interface IProductService
{
    Task<bool> RelatedToStoreAsync(int productId, int storeId);
    Task<bool> DeleteProductStoreRelation(int productId);
}
