using Ecommerce.Core.Common;

namespace Ecommerce.Core.Entities;

public class ProductStore : BaseEntity
{
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
    public int StoreId { get; set; }
    public Store Store { get; set; } = null!;
    public int Quantity { get; set; } = 0;

    public ProductStore(){}
    public ProductStore(
        int productId,
        int storeId,
        int quantity)
    {
        ProductId = productId;
        StoreId = storeId;
        Quantity = quantity;
    }

    public void IncreaseQuantity(int amountToIncrease = 1)
    {
        if (amountToIncrease < 1) throw new ArgumentException("Amount could not be less than 1");

        Quantity = Quantity + amountToIncrease;
    }

    public void DecreaseQuantity(int amountToDecrease = 1)
    {
        if (amountToDecrease < 1) throw new ArgumentException("Amount to decrease could not be less than 1");
        if (amountToDecrease > Quantity) throw new InvalidOperationException("Amount to decrease could not be greater than Quantity");

        Quantity = Quantity - amountToDecrease;
    }
}
