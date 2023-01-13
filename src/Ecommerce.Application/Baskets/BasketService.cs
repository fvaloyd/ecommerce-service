using Ecommerce.Application.Data;
using Ecommerce.Application.Stores;
using Ecommerce.Core.Entities;

using Microsoft.EntityFrameworkCore;
using Francisvac.Result;

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

    public async Task<Result> RestoreTheQuantityIntoStore(Basket basket)
    {
        Store store = await _db.Stores.FirstAsync();

        ProductStore? productStore = await _db.ProductStores.FirstOrDefaultAsync(s => s.ProductId == basket.ProductId && s.StoreId == store.Id);

        if (productStore is null) return Result.NotFound($"No product with id {basket.ProductId} is related to the store.");

        productStore.IncreaseQuantity(basket.Quantity);

        if (await _db.SaveChangesAsync() < 1) return Result.Error("Changes were not saved to the database.");

        return Result.Success("the product was restored to the store successfully.");
    }

    public async Task<Result> AddProductAsync(int productId, string userId)
    {
        Store store = await _db.Stores.FirstAsync();

        ProductStore? productStore = await _db.ProductStores
                                            .Include(ps => ps.Product)
                                            .FirstOrDefaultAsync(ps => ps.ProductId == productId && ps.StoreId == store.Id);
   
        if (productStore is null || productStore.Quantity == 0) return Result.Error("No products in stock.");

        Basket? userBasketExist = await _db.Baskets
                                            .FirstOrDefaultAsync(b => b.ProductId == productId && b.ApplicationUserId == userId);
        
        if (userBasketExist is not null) return Result.Error("The basket already contains the product.");

        Basket userBasket = new(productId, productStore.Product, userId);

        await _db.Baskets.AddAsync(userBasket);

        bool decreaseProductFromStoreResult = await _storeService.DecreaseProductAsync(productId, store.Id);

        if (!decreaseProductFromStoreResult) return Result.Error("An error occurred when trying to decrease the product in the store.");

        return Result.Success("Product added to basket satisfactorily.");
    }

    public async Task<Result> IncreaseProduct(int productId, string userId)
    {
        Store store = await _db.Stores.FirstAsync();

        Basket? userBasket = await _db.Baskets.Include(b => b.Product).FirstOrDefaultAsync(b => b.ProductId == productId && b.ApplicationUserId == userId);

        if (userBasket is null) return Result.NotFound($"The product with id {productId} is not found in the basket.");

        bool decreaseProductFromStoreResult = await _storeService.DecreaseProductAsync(productId: productId, storeId: store.Id);

        if (decreaseProductFromStoreResult is false) return Result.Error("An error occurred when trying to decrease the product in the store.");

        userBasket.IncreaseProductQuantity();

        if (await _db.SaveChangesAsync() < 1) return Result.Error("Changes were not saved to the database.");

        return Result.Success("The product was successfully increased.");
    }

    public async Task<Result> DecreaseProduct(int productId, string userId)
    {
        Store store = await _db.Stores.FirstAsync();

        Basket? userBasket = await _db.Baskets
                                    .Include(b => b.Product)                            
                                    .FirstOrDefaultAsync(b => b.ProductId == productId && b.ApplicationUserId == userId);

        if (userBasket is null) return Result.NotFound($"The basket does not contain the product with id {productId}.");

        bool increaseProductFromStoreResult = await _storeService.IncreaseProductAsync(productId: productId, storeId: store.Id);

        if (increaseProductFromStoreResult is false) return Result.Error("An error occurred when trying to increase the product in the store.");

        userBasket.DecreaseProductQuantity();

        if (await _db.SaveChangesAsync() < 1) return Result.Error("Changes were not saved to the database.");

        return Result.Success("The product was successfully increased.");
    }

    public async Task<Result<(IEnumerable<Product>, float)>> GetAllProducts(string userId)
    {
        List<Basket> userBasket = await _db.Baskets
                                                .Include(b => b.Product)
                                                .ThenInclude(p => p.Brand)
                                                .Include(b => b.Product)
                                                .ThenInclude(p => p.Category)
                                                .Where(b => b.ApplicationUserId == userId)
                                                .ToListAsync();

        if (userBasket is null || userBasket.Count < 1) return Result.NotFound("No product was found in the basket.");

        List<Product> userBasketProducts = userBasket.Select(sb => sb.Product).ToList();

        float total = userBasket.Select(ub => ub.Total).Sum();

        return (userBasketProducts, total);
    }

    public async Task<Result> RemoveProduct(int productId, string UserId)
    {
        Basket? userBasket = await _db.Baskets
                                        .Include(b => b.Product)
                                        .FirstOrDefaultAsync(b => b.ApplicationUserId == UserId && b.ProductId == productId);

        if (userBasket is null) return Result.NotFound("The product was not found in the basket.");

        Result restoreQuantityIntoStoreResult = await RestoreTheQuantityIntoStore(userBasket);

        if (restoreQuantityIntoStoreResult.IsSuccess) return restoreQuantityIntoStoreResult;

        _db.Baskets.Remove(userBasket);

        if (await _db.SaveChangesAsync() < 1) return Result.Error("Changes were not saved to the database.");

        return Result.Success("The product was removed from the basket successfully.");
    }
}
