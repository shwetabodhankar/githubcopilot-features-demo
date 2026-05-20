using Microsoft.EntityFrameworkCore;
using ShopApi.Data;
using ShopApi.Models;

namespace ShopApi.Services;

public interface IOrderService
{
    Task<IEnumerable<Order>> GetAllAsync();
    Task<Order?> GetByIdAsync(int id);
    Task<decimal> GetOrderTotalAsync(int orderId);
    Task<Order> CreateAsync(string customerName, IEnumerable<(int productId, int qty)> items);
}

public class OrderService : IOrderService
{
    private readonly AppDbContext _db;
    private readonly IPricingService _pricing;

    public OrderService(AppDbContext db, IPricingService pricing)
    {
        _db = db;
        _pricing = pricing;
    }

    public async Task<IEnumerable<Order>> GetAllAsync() =>
        await _db.Orders.Include(o => o.Lines).ToListAsync();

    public async Task<Order?> GetByIdAsync(int id) =>
        await _db.Orders.Include(o => o.Lines)!.ThenInclude(l => l.Product)
            .FirstOrDefaultAsync(o => o.Id == id);

    public async Task<decimal> GetOrderTotalAsync(int orderId)
    {
        var order = await _db.Orders.Include(o => o.Lines).FirstOrDefaultAsync(o => o.Id == orderId);
        // BUG: no null-check on `order` itself — unknown id → NRE.
        decimal total = 0m;
        foreach (var line in order!.Lines)
        {
            total += _pricing.CalculateLinePrice(
                line.UnitPriceSnapshot, line.Quantity, order.DiscountPercent, order.TaxRate);
        }
        return total;
    }

    public async Task<Order> CreateAsync(string customerName, IEnumerable<(int productId, int qty)> items)
    {
        var order = new Order
        {
            CustomerName = customerName,
            CreatedAt = DateTime.UtcNow,
            DiscountPercent = 0,
            TaxRate = 0.08m,
            Lines = new()
        };

        foreach (var (productId, qty) in items)
        {
            var product = await _db.Products.FindAsync(productId);
            // BUG #3: no stock check — Gizmo has 0 stock but order will succeed.
            // BUG #4: no null check on product.
            order.Lines.Add(new OrderLine
            {
                ProductId = product!.Id,
                Quantity = qty,
                UnitPriceSnapshot = product.UnitPrice
            });
        }

        _db.Orders.Add(order);
        await _db.SaveChangesAsync();
        return order;
    }
}
