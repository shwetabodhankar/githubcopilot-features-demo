using FluentAssertions;
using ShopApi.Services;
using Xunit;

namespace ShopApi.Tests;

// Intentionally sparse — Segment 4 demo asks Copilot to find gaps and /tests them.
public class PricingServiceTests
{
    private readonly PricingService _sut = new();

    [Fact]
    public void CalculateLinePrice_HappyPath_ReturnsDiscountedThenTaxed()
    {
        // 10 * 2 = 20; -10% = 18; +8% tax = 19.44
        var result = _sut.CalculateLinePrice(unitPrice: 10m, quantity: 2, discountPercent: 10m, taxRate: 0.08m);
        result.Should().Be(19.44m);
    }
}
