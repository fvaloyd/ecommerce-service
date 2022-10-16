using Ecommerce.Core.Entities;
using Ecommerce.Core.Interfaces;

namespace Ecommerce.Core.Services;

public class ProductService : IProductService
{
    private readonly IEfRepository<Product> _productRepo;
    private readonly IEfRepository<Store> _storeRepo;
    private readonly IEfRepository<ProductStore> _storeProductRepo;

    public ProductService(
        IEfRepository<Product> productRepo,
        IEfRepository<Store> storeRepo,
        IEfRepository<ProductStore> storeProductRepo)
    {
        _productRepo = productRepo;
        _storeRepo = storeRepo;
        _storeProductRepo = storeProductRepo;
    }

    public async Task DeleteProductStoreRelation(int productId)
    {
        var storeProduct = await _storeProductRepo.GetAllAsync(sp => sp.ProductId == productId);

        if (storeProduct is null)
            return;

        _storeProductRepo.RemoveRange(storeProduct);
    }

    public async Task<int> RelatedToStoreAsync(int productId, int storeId)
    {
        var storeProduct = _storeProductRepo.GetFirst(sp => sp.StoreId == storeId && sp.ProductId == productId);

        if (storeProduct is not null)
        {
            storeProduct.IncreaseQuantity();
            return await _storeProductRepo.SaveChangeAsync();
        }

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
