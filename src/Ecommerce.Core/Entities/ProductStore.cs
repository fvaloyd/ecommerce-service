namespace Ecommerce.Core.Entities;

public class ProductStore : BaseEntity
{
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
    public int StoreId { get; set; }
    public Store Store { get; set; } = null!;
    public int Quantity { get; set; } = 0;

    public void AddProduct(int storeId, int productId)
    {
        StoreId = storeId;
        ProductId = productId;
    }

    public void IncreaseQuantity(int quantity = 1)
    {
        if (quantity > 1)
        {
            Quantity += quantity;
            return;
        }
        Quantity++;
    }
    public void DecreaseQuantity()
    {
        if (Quantity == 0)
        {
            return;
        }
        Quantity--;
    }
}
