using Microsoft.AspNetCore.Mvc;
using ShopApi.Services;

namespace ShopApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orders;
    public OrdersController(IOrderService orders) => _orders = orders;

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _orders.GetAllAsync());

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var o = await _orders.GetByIdAsync(id);
        if (o is null) return NotFound();
        var total = await _orders.GetOrderTotalAsync(id);
        // BUG #1 demo: .First() throws InvalidOperationException when Lines is empty
        // (Bob's seeded order has no lines). Clicking order #2 in the UI returns 500.
        var firstSku = o.Lines.First().Product!.Name;
        return Ok(new
        {
            o.Id, o.CustomerName, o.CreatedAt, o.DiscountPercent, o.TaxRate, o.Lines,
            FirstSku = firstSku,
            Total = total
        });
    }

    public record CreateOrderRequest(string CustomerName, List<LineDto> Items);
    public record LineDto(int ProductId, int Qty);

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateOrderRequest req)
    {
        var order = await _orders.CreateAsync(req.CustomerName, req.Items.Select(i => (i.ProductId, i.Qty)));
        return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
    }
}
