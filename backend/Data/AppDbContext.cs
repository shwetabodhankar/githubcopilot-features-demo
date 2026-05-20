using Microsoft.EntityFrameworkCore;
using ShopApi.Models;

namespace ShopApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> opt) : base(opt) { }

    public DbSet<Product> Products => Set<Product>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderLine> OrderLines => Set<OrderLine>();
}

public static class DbSeeder
{
    public static void Seed(AppDbContext db)
    {
        if (db.Products.Any()) return;

        var p1 = new Product { Name = "Widget",  UnitPrice = 9.99m,  StockOnHand = 100 };
        var p2 = new Product { Name = "Gadget",  UnitPrice = 19.50m, StockOnHand = 40 };
        var p3 = new Product { Name = "Gizmo",   UnitPrice = 4.25m,  StockOnHand = 0 };
        db.Products.AddRange(p1, p2, p3);
        db.SaveChanges();

        db.Orders.AddRange(
            new Order
            {
                CustomerName = "Alice",
                CreatedAt = DateTime.UtcNow.AddDays(-2),
                DiscountPercent = 10m,
                TaxRate = 0.08m,
                Lines = new()
                {
                    new OrderLine { ProductId = p1.Id, Quantity = 2, UnitPriceSnapshot = p1.UnitPrice },
                    new OrderLine { ProductId = p2.Id, Quantity = 1, UnitPriceSnapshot = p2.UnitPrice },
                }
            },
            // BUG demo: this order has NO lines — GetOrderTotal throws NRE on it.
            new Order
            {
                CustomerName = "Bob",
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                DiscountPercent = 0m,
                TaxRate = 0.08m,
                Lines = null!
            }
        );
        db.SaveChanges();
    }
}
