using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopApi.Data;

namespace ShopApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly AppDbContext _db;
    public ProductsController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _db.Products.ToListAsync());

    // BUG demo: builds SQL via string interpolation — perfect /fix target for security prompt.
    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string name)
    {
        var sql = $"SELECT * FROM Products WHERE Name LIKE '%{name}%'";
        var results = await _db.Products.FromSqlRaw(sql).ToListAsync();
        return Ok(results);
    }
}
