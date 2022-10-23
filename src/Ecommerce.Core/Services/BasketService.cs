using Ecommerce.Core.Entities;
using Ecommerce.Core.Interfaces;

namespace Ecommerce.Core.Services;

public class BasketService : IBasketService
{
    private readonly IEfRepository<Basket> _basketRepo;
    private readonly IEfRepository<Store> _storeRepo;
    private readonly IEfRepository<ProductStore> _productStoreRepo;
    private readonly IStoreService _storeService;

    public BasketService(
        IEfRepository<Store> storeRepo,
        IEfRepository<Basket> basketRepo,
        IEfRepository<ProductStore> productStoreRepo,
        IStoreService storeService)
    {
        _storeRepo = storeRepo;
        _basketRepo = basketRepo;
        _productStoreRepo = productStoreRepo;
        _storeService = storeService;
    }

    public async Task<bool> RestoreTheQuantityIntoStore(Basket basket)
    {
        Store store = _storeRepo.GetFirst();

        ProductStore productStore = _productStoreRepo.GetFirst(s => s.ProductId == basket.Product.Id && s.StoreId == store.Id);

        if (productStore is null) return false;       

        productStore.IncreaseQuantity(basket.Quantity);

        int rowsAffectInThePersistence = await _productStoreRepo.SaveChangeAsync();

        if (rowsAffectInThePersistence < 1) return false;

        return true;
    }

    public async Task<bool> AddProductAsync(int productId, string userId)
    {
        Store store = _storeRepo.GetFirst();

        ProductStore productInStock = _productStoreRepo.GetFirst(s => s.ProductId == productId && s.StoreId == store.Id, IncludeProperty: "Product");
        if (productInStock is null) return false;

        Basket userBasketExist = _basketRepo.GetFirst(b => b.ProductId == productId && b.ApplicationUserId == userId);
        if (userBasketExist is not null) return false;

        Basket userBasket = new Basket()
        {
            ProductId = productId,
            ApplicationUserId = userId,
            Quantity = 1,
            Total = 1 * productInStock.Product.Price
        };

        Basket basketCreated = await _basketRepo.AddAsync(userBasket);

        if (basketCreated is null) return false;

        bool decreaseProductFromStoreResult = await _storeService.DecreaseProductAsync(productId, store.Id);

        if (decreaseProductFromStoreResult is false)
        {
            _basketRepo.Remove(basketCreated);
            return false;
        }

        return true;
    }

    public async Task<bool> IncreaseProduct(int productId, string userId)
    {
        Store store = _storeRepo.GetFirst();

        Basket userBasket = _basketRepo.GetFirst(b => b.ProductId == productId && b.ApplicationUserId == userId, IncludeProperty: "Product");

        if (userBasket is null) return false;
        
        bool decreaseProductFromStoreResult = await _storeService.DecreaseProductAsync(productId: productId, storeId: store.Id);

        if (decreaseProductFromStoreResult is false) return false;

        userBasket.IncreaseProductQuantity();
        userBasket.IncreaseTotal(userBasket.Product.Price);

        int rowsAffectInThePersistence = await _basketRepo.SaveChangeAsync();

        if (rowsAffectInThePersistence < 1) return false;

        return true;
    }

    public async Task<bool> DecreaseProduct(int productId, string userId)
    {
        Store store = _storeRepo.GetFirst();

        Basket userBasket = _basketRepo.GetFirst(b => b.ProductId == productId && b.ApplicationUserId == userId, IncludeProperty: "Product");

        if (userBasket is null) return false;

        bool increaseProductFromStoreResult = await _storeService.IncreaseProductAsync(productId: productId, storeId: store.Id);

        if (increaseProductFromStoreResult is false) return false;

        userBasket.DecreaseProductQuantity(1);
        userBasket.DecreaseTotal(userBasket.Product.Price);

        int rowsAffectInThePersistence = await _basketRepo.SaveChangeAsync();

        if (rowsAffectInThePersistence < 1) return false;

        return true;
    }

    public async Task<IEnumerable<Product>> GetAllProducts(string userId)
    {
        Store store = _storeRepo.GetFirst();

        IEnumerable<Basket> userBaskets = await _basketRepo.GetAllAsync(b => b.ApplicationUserId == userId, IncludeProperty: "Product");

        if (userBaskets is null) throw new InvalidOperationException("The user did not have a basket associated");

        IEnumerable<Product> userBasketProducts = userBaskets.Select(sb => sb.Product).ToList();

        return userBasketProducts;
    }
}
