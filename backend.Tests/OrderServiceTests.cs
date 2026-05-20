using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using ShopApi.Data;
using ShopApi.Models;
using ShopApi.Services;
using Xunit;

namespace ShopApi.Tests;

// Only the happy-path test exists — the "no lines" bug is uncovered.
// Demo target for Segment 3 (/fix) and Segment 4 (/tests).
public class OrderServiceTests
{
    private static AppDbContext NewDb()
    {
        var opts = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        return new AppDbContext(opts);
    }

    [Fact]
    public async Task GetOrderTotalAsync_WithLines_ReturnsExpectedTotal()
    {
        using var db = NewDb();
        db.Orders.Add(new Order
        {
            CustomerName = "Test",
            DiscountPercent = 0,
            TaxRate = 0,
            Lines = new()
            {
                new OrderLine { Quantity = 2, UnitPriceSnapshot = 5m },
                new OrderLine { Quantity = 1, UnitPriceSnapshot = 3m },
            }
        });
        await db.SaveChangesAsync();

        var sut = new OrderService(db, new PricingService());
        var total = await sut.GetOrderTotalAsync(1);

        total.Should().Be(13m);
    }
}
