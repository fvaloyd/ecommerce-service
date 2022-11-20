namespace Ecommerce.Core.Entities;

public class OrderDetail : BaseEntity
{
    public int OrderId { get; set; }
    public string ApplicationUserId { get; set; } = null!;
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
    public int Quantity { get; set; }
    public double UnitPrice { get; set; }
    public OrderDetail(){}
    public OrderDetail(int orderId, string applicationUserId, int productId,
    int quantity, double unitPrice)
    {
        OrderId = orderId;
        ApplicationUserId = applicationUserId;
        ProductId = productId;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }
}