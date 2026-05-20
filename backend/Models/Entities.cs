namespace ShopApi.Models;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public decimal UnitPrice { get; set; }
    public int StockOnHand { get; set; }
}

public class Order
{
    public int Id { get; set; }
    public string CustomerName { get; set; } = "";
    public DateTime CreatedAt { get; set; }
    public List<OrderLine> Lines { get; set; } = new();
    public decimal DiscountPercent { get; set; }
    public decimal TaxRate { get; set; }
}

public class OrderLine
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public int ProductId { get; set; }
    public Product? Product { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPriceSnapshot { get; set; }
}
