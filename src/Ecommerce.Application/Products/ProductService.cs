using Ecommerce.Application.Data;
using Ecommerce.Core.Entities;
using Francisvac.Result;
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

    public async Task<Result> DeleteProductStoreRelation(int productId)
    {
        List<ProductStore>? storeProducts = await _db.ProductStores.Where(sp => sp.ProductId == productId).ToListAsync();

        if (storeProducts is null || storeProducts.Count < 1) return Result.NotFound("No relationship of the product was found with any store.");

        _db.ProductStores.RemoveRange(storeProducts);

        if (await _db.SaveChangesAsync() < 1) return Result.Error("Changes were not saved to the database.");

        return Result.Success("Removed the product from the system successfully.");
    }

    public async Task<Result> RelatedToStoreAsync(int productId, int storeId)
    {
        var storeProduct = await _db.ProductStores.FirstOrDefaultAsync(sp => sp.StoreId == storeId && sp.ProductId == productId);

        if (storeProduct is not null) return Result.Error("The product is already related to the store.");

        storeProduct = new ProductStore(productId, storeId);

        await _db.ProductStores.AddAsync(storeProduct);

        if (await _db.SaveChangesAsync() < 1) return Result.Error("Changes were not saved to the database.");

        return Result.Success("The product was added to the store successfully.");
    }
}
