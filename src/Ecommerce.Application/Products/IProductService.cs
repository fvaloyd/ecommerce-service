using Francisvac.Result;

namespace Ecommerce.Application.Products;

public interface IProductService
{
    Task<Result> RelatedToStoreAsync(int productId, int storeId);
    Task<Result> DeleteProductStoreRelation(int productId);
}
