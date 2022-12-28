using Ecommerce.Core.Common;

namespace Ecommerce.Core.Entities;

public class Basket
{
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
    public string ApplicationUserId { get; set; } = null!;
    public int Quantity { get; set; }
    public float Total { get; set; }

    public void IncreaseProductQuantity(int quantity = 1)
    {
        if (quantity < 1) throw new ArgumentException("Amount to increase could not be less than 1");

        Quantity += quantity;

        ReloadTotal();
    }

    private void ReloadTotal()
    {
        if (Product is null) throw new InvalidOperationException("Product could not be null when reload the total");

        Total = Product.Price * Quantity;
    }

    public void DecreaseProductQuantity(int amountToDecrease = 1)
    {
        if (Quantity == 0) throw new InvalidOperationException("The Basket doesn't have a quantity to decrease");

        if (amountToDecrease < 1) throw new ArgumentException("Amount to decrease could not be less than 1");

        Quantity -= amountToDecrease;

        ReloadTotal();
    }

    public Basket(){}

    public Basket(int productId, string applicationUserId, int quantity = 1)
    {
        ProductId = productId;

        ApplicationUserId = applicationUserId;

        Quantity = quantity;
    }

    public Basket(int productId, Product product, string applicationUserId, int quantity = 1)
    {
        ProductId = productId;
        ApplicationUserId = applicationUserId;
        Quantity = quantity;
        if (product is not null)
        {
            Product = product;
            ReloadTotal();
        }
    }
}
