namespace Ecommerce.Core.Entities;

public class OrderDetail : BaseEntity
{
    public int OrderId { get; set; }
    public string UserName { get; set; } = null!;
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
    public int Quantity { get; set; }
    public double UnitPrice { get; set; }
    public OrderDetail(){}
    public OrderDetail(int orderId, string userName, int productId,
    int quantity, double unitPrice)
    {
        OrderId = orderId;
        UserName = userName;
        ProductId = productId;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }
}