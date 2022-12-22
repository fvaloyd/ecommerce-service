using Ecommerce.Application.Data;
using Ecommerce.Application.Stores;
using Ecommerce.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Application.Baskets;

public class BasketService : IBasketService
{
    private readonly IStoreService _storeService;
    private readonly IEcommerceDbContext _db;

    public BasketService(
        IStoreService storeService,
        IEcommerceDbContext db)
    {
        _storeService = storeService;
        _db = db;
    }

    public async Task<bool> RestoreTheQuantityIntoStore(Basket basket)
    {
        Store store = await _db.Stores.FirstAsync();

        ProductStore? productStore = await _db.ProductStores.FirstOrDefaultAsync(s => s.ProductId == basket.ProductId && s.StoreId == store.Id);

        if (productStore is null) return false;

        productStore.IncreaseQuantity(basket.Quantity);

        if (await _db.SaveChangesAsync() < 1) return false;

        return true;
    }

    public async Task<bool> AddProductAsync(int productId, string userId)
    {
        Store store = await _db.Stores.FirstAsync();

        ProductStore? productStore = await _db.ProductStores
                                            .Include(ps => ps.Product)
                                            .FirstOrDefaultAsync(ps => ps.ProductId == productId && ps.StoreId == store.Id);
   
        if (productStore is null || productStore.Quantity == 0) return false;

        Basket? userBasketExist = await _db.Baskets
                                            .FirstOrDefaultAsync(b => b.ProductId == productId && b.ApplicationUserId == userId);
        
        if (userBasketExist is not null) return false;

        Basket userBasket = new(productId, productStore.Product, userId);

        await _db.Baskets.AddAsync(userBasket);

        bool decreaseProductFromStoreResult = await _storeService.DecreaseProductAsync(productId, store.Id);

        if (decreaseProductFromStoreResult is false)
        {
            _db.Baskets.Remove(userBasket);

            return false;
        }

        await _db.SaveChangesAsync();

        return true;
    }

    public async Task<bool> IncreaseProduct(int productId, string userId)
    {
        Store store = await _db.Stores.FirstAsync();

        Basket? userBasket = await _db.Baskets.Include(b => b.Product).FirstOrDefaultAsync(b => b.ProductId == productId && b.ApplicationUserId == userId);

        if (userBasket is null) return false;

        bool decreaseProductFromStoreResult = await _storeService.DecreaseProductAsync(productId: productId, storeId: store.Id);

        if (decreaseProductFromStoreResult is false) return false;

        userBasket.IncreaseProductQuantity();

        if (await _db.SaveChangesAsync() < 1) return false;

        return true;
    }

    public async Task<bool> DecreaseProduct(int productId, string userId)
    {
        Store store = await _db.Stores.FirstAsync();

        Basket? userBasket = await _db.Baskets
                                    .Include(b => b.Product)                            
                                    .FirstOrDefaultAsync(b => b.ProductId == productId && b.ApplicationUserId == userId);

        if (userBasket is null) return false;

        bool increaseProductFromStoreResult = await _storeService.IncreaseProductAsync(productId: productId, storeId: store.Id);

        if (increaseProductFromStoreResult is false) return false;

        userBasket.DecreaseProductQuantity();

        if (await _db.SaveChangesAsync() < 1) return false;

        return true;
    }

    public async Task<(IEnumerable<Product>, float)> GetAllProducts(string userId)
    {
        IEnumerable<Basket> userBasket = _db.Baskets
                                                .Include(b => b.Product)
                                                .ThenInclude(p => p.Brand)
                                                .Include(b => b.Product)
                                                .ThenInclude(p => p.Category)
                                                .Where(b => b.ApplicationUserId == userId);

        if (userBasket is null) throw new InvalidOperationException("The user did not have a basket associated");

        IEnumerable<Product> userBasketProducts = userBasket.Select(sb => sb.Product).ToList();

        float total = userBasket.Select(ub => ub.Total).Sum();

        return (userBasketProducts, total);
    }

    public async Task<bool> RemoveProduct(int productId, string UserId)
    {
        Basket? userBasket = await _db.Baskets
                                        .Include(b => b.Product)
                                        .FirstOrDefaultAsync(b => b.ApplicationUserId == UserId && b.ProductId == productId);

        if (userBasket is null) return false;

        if (await RestoreTheQuantityIntoStore(userBasket) is false) return false;

        _db.Baskets.Remove(userBasket);

        if (await _db.SaveChangesAsync() < 1) return false;

        return true;
    }
}
