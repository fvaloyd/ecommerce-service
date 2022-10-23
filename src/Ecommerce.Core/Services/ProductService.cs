using Ecommerce.Core.Entities;
using Ecommerce.Core.Interfaces;

namespace Ecommerce.Core.Services;

public class ProductService : IProductService
{
    private readonly IEfRepository<ProductStore> _storeProductRepo;

    public ProductService(
        IEfRepository<ProductStore> storeProductRepo)
    {
        _storeProductRepo = storeProductRepo;
    }

    public async Task DeleteProductStoreRelation(int productId)
    {
        var storeProduct = await _storeProductRepo.GetAllAsync(sp => sp.ProductId == productId);

        if (storeProduct is null) throw new InvalidOperationException($"Store doesnt have a product with Id: ${productId}");

        _storeProductRepo.RemoveRange(storeProduct);
    }

    public async Task<int> RelatedToStoreAsync(int productId, int storeId)
    {
        var storeProduct = _storeProductRepo.GetFirst(sp => sp.StoreId == storeId && sp.ProductId == productId);

        if (storeProduct is not null) throw new InvalidOperationException("The product is already related to the store.");

        storeProduct = new ProductStore()
        {
            ProductId = productId,
            StoreId = storeId,
        };

        var storeProductCreated = await _storeProductRepo.AddAsync(storeProduct);

        if (storeProductCreated is null) return 0;

        return 1;
    }
}
