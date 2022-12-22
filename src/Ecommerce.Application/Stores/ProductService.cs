using Ecommerce.Application.Common.Interfaces;
using Ecommerce.Core.Entities;

namespace Ecommerce.Application.Stores;

public class ProductService : IProductService
{
    private readonly IEfRepository<ProductStore> _storeProductRepo;

    public ProductService(
        IEfRepository<ProductStore> storeProductRepo)
    {
        _storeProductRepo = storeProductRepo;
    }

    public void DeleteProductStoreRelation(int productId)
    {
        var storeProduct = _storeProductRepo.GetAll(sp => sp.ProductId == productId);

        if (storeProduct is null) throw new InvalidOperationException($"Store doesnt have a product with Id: ${productId}");

        _storeProductRepo.RemoveRange(storeProduct);
    }

    public async Task<bool> RelatedToStoreAsync(int productId, int storeId)
    {
        var storeProduct = _storeProductRepo.GetFirst(sp => sp.StoreId == storeId && sp.ProductId == productId);

        if (storeProduct is not null) throw new InvalidOperationException("The product is already related to the store.");

        storeProduct = new ProductStore()
        {
            ProductId = productId,
            StoreId = storeId,
        };

        var storeProductCreated = await _storeProductRepo.AddAsync(storeProduct);

        if (storeProductCreated is null) return false;

        return true;
    }
}
