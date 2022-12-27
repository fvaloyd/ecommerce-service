using Ecommerce.Application.Common.Interfaces;
using Ecommerce.Application.Data;
using Ecommerce.Core.Entities;

using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Application.Stores;

public class StoreService : IStoreService
{
    private readonly IEcommerceDbContext _db;

    public StoreService(
        IEcommerceDbContext db)
    {
        _db = db;
    }

    public async Task<bool> AddProductAsync(int productId, int storeId)
    {
        if (await _db.ProductStores.AnyAsync(ps => ps.ProductId == productId && ps.StoreId == storeId)) return false;

        await _db.ProductStores.AddAsync(new ProductStore(productId, storeId));

        if (await _db.SaveChangesAsync() < 1) return false;

        return true;
    }

    public async Task<bool> DecreaseProductAsync(int productId, int storeId)
    {
        ProductStore? productStore = await _db.ProductStores.FirstOrDefaultAsync(ps => ps.StoreId == storeId && ps.ProductId == productId);

        if (productStore is null) return false;

        if (productStore.Quantity <= 1)
        {
            _db.ProductStores.Remove(productStore);

            if (await _db.SaveChangesAsync() < 1) return false;

            return true;
        }

        if (await _db.SaveChangesAsync() < 1) return false;

        return true;
    }

    public async Task<bool> DeleteProductStoreRelation(int storeId)
    {
        var productStores = await _db.ProductStores.Where(ps => ps.StoreId.Equals(storeId)).ToListAsync();

        if (productStores.Count < 1) return false;

        _db.ProductStores.RemoveRange(productStores);

        if (await _db.SaveChangesAsync() < 1) return false;

        return true;
    }

    public async Task<bool> IncreaseProductAsync(int productId, int storeId)
    {
        var storeProduct = await _db.ProductStores.FirstOrDefaultAsync(ps => ps.StoreId == storeId && ps.ProductId == productId);

        if (storeProduct is null) return false;

        storeProduct.IncreaseQuantity();

        if (await _db.SaveChangesAsync() < 1) return false;

        return true;
    }
}
