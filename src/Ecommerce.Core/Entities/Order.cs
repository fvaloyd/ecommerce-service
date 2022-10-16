namespace Ecommerce.Core.Entities;

public class Order : BaseEntity
{
    public int Id { get; set; }
    public DateTime OrderDate { get; set; }
    public string ApplicationUserId { get; set; } = null!;
    public decimal Total { get; set; }
    public string PaymentTransactionId { get; set; } = null!;
    public List<OrderDetail> OrderDetails { get; set; } = null!;
}