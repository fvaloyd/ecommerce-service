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

    public void DecreaseProductQuantity(int quantity = 1)
    {
        if (Quantity == 0)
        {
            return;
        }

        Quantity -= quantity;
    }
}
