namespace ShopApi.Services;

public interface IPricingService
{
    // Calculate the final price for an order line:
    // - apply percentage discount (0-100) BEFORE tax
    // - apply flat tax rate (0-1) AFTER discount
    // - never return negative; throw ArgumentOutOfRangeException on bad input
    decimal CalculateLinePrice(decimal unitPrice, int quantity, decimal discountPercent, decimal taxRate);
}

public class PricingService : IPricingService
{
    public decimal CalculateLinePrice(decimal unitPrice, int quantity, decimal discountPercent, decimal taxRate)
    {
        // Happy-path-only implementation. Demo target for /tests and /fix.
        var subtotal = unitPrice * quantity;
        var discounted = subtotal - (subtotal * discountPercent / 100m);
        var taxed = discounted + (discounted * taxRate);
        return taxed;
    }
}
