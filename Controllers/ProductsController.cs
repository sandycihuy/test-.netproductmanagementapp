using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductManagementApp.Data;
using ProductManagementApp.Models;
using System.Security.Claims;

namespace ProductManagementApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProductsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return await _context.Products
                .Where(p => p.UserId == userId && !p.IsDeleted)
                .Include(p => p.Category)
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId && !p.IsDeleted);

            if (product == null)
            {
                return NotFound();
            }

            return product;
        }

        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct(Product product)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            product.UserId = userId;
            product.CreatedAt = DateTime.UtcNow;

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(int id, Product product)
        {
            if (id != product.Id)
            {
                return BadRequest();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (product.UserId != userId)
            {
                return Unauthorized();
            }

            _context.Entry(product).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId && !p.IsDeleted);

            if (product == null)
            {
                return NotFound();
            }

            product.IsDeleted = true;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}