using Ecommerce.Application.Data;
using Ecommerce.Core.Entities;

using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Application.Products;

public class ProductService : IProductService
{
    private readonly IEcommerceDbContext _db;

    public ProductService(
        IEcommerceDbContext db)
    {
        _db = db;
    }

    public async Task<bool> DeleteProductStoreRelation(int productId)
    {
        List<ProductStore>? storeProducts = await _db.ProductStores.Where(sp => sp.ProductId == productId).ToListAsync();

        if (storeProducts is null || storeProducts.Count < 1) return false;

        _db.ProductStores.RemoveRange(storeProducts);

        if (await _db.SaveChangesAsync() < 1) return false;

        return true;
    }

    public async Task<bool> RelatedToStoreAsync(int productId, int storeId)
    {
        var storeProduct = await _db.ProductStores.FirstOrDefaultAsync(sp => sp.StoreId == storeId && sp.ProductId == productId);

        if (storeProduct is not null) return false;

        storeProduct = new ProductStore(productId, storeId);

        await _db.ProductStores.AddAsync(storeProduct);

        if (await _db.SaveChangesAsync() < 1) return false;

        return true;
    }
}
