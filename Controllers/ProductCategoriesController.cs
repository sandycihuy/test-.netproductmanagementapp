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
    public class ProductCategoriesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ProductCategoriesController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductCategory>>> GetProductCategories()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return await _context.ProductCategories
                .Where(pc => pc.UserId == userId && !pc.IsDeleted)
                .ToListAsync();
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<ProductCategory>> GetProductCategory(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var productCategory = await _context.ProductCategories
                .FirstOrDefaultAsync(pc => pc.Id == id && pc.UserId == userId && !pc.IsDeleted);

            if (productCategory == null)
            {
                return NotFound();
            }

            return productCategory;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutProductCategory(int id, ProductCategory productCategory)
        {
            if (id != productCategory.Id)
            {
                return BadRequest();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var existingCategory = await _context.ProductCategories
                .FirstOrDefaultAsync(pc => pc.Id == id && pc.UserId == userId);

            if (existingCategory == null)
            {
                return NotFound();
            }

            existingCategory.Name = productCategory.Name;
            existingCategory.Description = productCategory.Description;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductCategoryExists(id))
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


        [HttpPost]
        public async Task<ActionResult<ProductCategory>> PostProductCategory(ProductCategory productCategory)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            productCategory.UserId = userId;
            productCategory.CreatedAt = DateTime.UtcNow;
            productCategory.IsDeleted = false;

            _context.ProductCategories.Add(productCategory);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProductCategory", new { id = productCategory.Id }, productCategory);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProductCategory(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var productCategory = await _context.ProductCategories
                .FirstOrDefaultAsync(pc => pc.Id == id && pc.UserId == userId && !pc.IsDeleted);

            if (productCategory == null)
            {
                return NotFound();
            }

            
            productCategory.IsDeleted = true;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProductCategoryExists(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return _context.ProductCategories.Any(e => e.Id == id && e.UserId == userId && !e.IsDeleted);
        }
    }
}
