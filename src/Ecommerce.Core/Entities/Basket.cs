namespace Ecommerce.Core.Entities;

public class Basket
{
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
    public string ApplicationUserId { get; set; } = null!;
    public int Quantity { get; set; }
    public float Total { get; set; }

    public int IncreaseProductQuantity(int quantity = 1)
    {
        if (quantity < 1) throw new ArgumentOutOfRangeException("The argument could not be less than 1");

        Quantity += quantity;

        ReloadTotal();

        return quantity;
    }

    private void ReloadTotal()
    {
        if (Product is null) throw new InvalidOperationException("Product could not be null when reload the total");

        Total = Product.Price * Quantity;
    }

    public int DecreaseProductQuantity(int amountToDecrease = 1)
    {
        if (amountToDecrease < 1) throw new ArgumentOutOfRangeException("The argument could not be less than 1");

        if (Quantity == 0) return 0;

        Quantity -= amountToDecrease;

        ReloadTotal();

        return amountToDecrease;
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
