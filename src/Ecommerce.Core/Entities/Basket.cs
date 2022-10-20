namespace Ecommerce.Core.Entities;

public class Basket : BaseEntity
{
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
    public string ApplicationUserId { get; set; } = null!;
    public int Quantity { get; set; }
    public float Total { get; set; }

    public void IncreaseProductQuantity(int quantity = 1)
    {
        if (quantity < 1) throw new ArgumentException("Quantity could not be less than 1");
        Quantity += quantity;
    }

    public void IncreaseTotal(float productPrice)
    {
        Total = productPrice * Quantity;
    }

    public void DecreaseTotal(float productPrice)
    {
        Total -= productPrice;
    }

    public void DecreaseProductQuantity(int amountToDecrease = 1)
    {
        if (Quantity == 0) throw new InvalidOperationException("The Basket doesn't have a quantity to decrease");

        if (amountToDecrease < 1) throw new ArgumentException("Amount to decrease could not be less than 1");

        Quantity -= amountToDecrease;
    }

    public Basket(){}

    public Basket(int productId, string applicationUserId, int quantity)
    {
        ProductId = productId;
        ApplicationUserId = applicationUserId;
        Quantity = quantity;
    }
}
