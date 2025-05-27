using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using technicalTestProject_1.Models;
using technicalTestProject_1.Data;
using System.Security.Claims;

[Route("/product")]
[Authorize]
public class ProductController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _hostEnvironment;

    public ProductController(ApplicationDbContext context, IWebHostEnvironment hostEnvironment)
    {
        _context = context;
        _hostEnvironment = hostEnvironment;
    }

    [HttpGet]
    public async Task<IActionResult> IndexProduct()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var products = await _context.Products
            .Where(p => p.CreatorId == userId) // Filter dulu
            .Include(p => p.Creator)
            .Include(p => p.Category)
            .Select(p => new ProductViewModel
            {
                Id = p.Id,
                ProductName = p.ProductName,
                ProductImage = p.ProductImage,
                Price = p.Price,
                CreatorId = p.CreatorId,
                CreatorName = p.Creator.FullName ?? "Unknown",
                CategoryId = p.CategoryId,
                CategoryName = p.Category.CategoryName,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt
            })
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();

        var categories = await _context.Categories.ToListAsync();
        ViewBag.Categories = categories;

        return View("~/Views/Product/IndexProduct.cshtml", products);
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchProducts(string searchTerm = "", DateTime? searchDate = null)
    {
        var query = _context.Products
            .Include(p => p.Creator)
            .Include(p => p.Category)
            .AsQueryable();

        if (!string.IsNullOrEmpty(searchTerm))
        {
            query = query.Where(p => p.ProductName.Contains(searchTerm));
        }

        if (searchDate.HasValue)
        {
            var date = searchDate.Value.Date;
            query = query.Where(p => p.CreatedAt.Date == date);
        }

        var products = await query
            .Select(p => new ProductViewModel
            {
                Id = p.Id,
                ProductName = p.ProductName,
                ProductImage = p.ProductImage,
                Price = p.Price,
                CreatorId = p.CreatorId,
                CreatorName = p.Creator.UserName ?? "Unknown",
                CategoryId = p.CategoryId,
                CategoryName = p.Category.CategoryName,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt
            })
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();

        return PartialView("~/Views/Product/_ProductTablePartial.cshtml", products);
    }

    [HttpPost("create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateProduct(CreateProductViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return Json(new { success = false, message = "Invalid data provided" });
        }

        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { success = false, message = "User not authenticated" });
            }

            var product = new Product
            {
                ProductName = model.ProductName,
                Price = model.Price,
                CategoryId = model.CategoryId,
                CreatorId = userId,
                CreatedAt = DateTime.UtcNow
            };

            // Handle image upload
            if (model.ProductImageFile != null && model.ProductImageFile.Length > 0)
            {
                product.ProductImage = await SaveProductImage(model.ProductImageFile);
            }
            else
            {
                product.ProductImage = "/images/default-product.jpg";
            }

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Product created successfully" });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = "Error creating product: " + ex.Message });
        }
    }

    [HttpPost("update/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateProduct(int id, UpdateProductViewModel model)
    {
        if (id != model.Id)
        {
            return Json(new { success = false, message = "Invalid product ID" });
        }

        if (!ModelState.IsValid)
        {
            return Json(new { success = false, message = "Invalid data provided" });
        }

        try
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return Json(new { success = false, message = "Product not found" });
            }

            // Check if user owns this product
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (product.CreatorId != userId)
            {
                return Json(new { success = false, message = "Unauthorized to edit this product" });
            }

            product.ProductName = model.ProductName;
            product.Price = model.Price;
            product.CategoryId = model.CategoryId;
            product.UpdatedAt = DateTime.UtcNow;

            // Handle image update
            if (model.ProductImageFile != null && model.ProductImageFile.Length > 0)
            {
                // Delete old image if it exists and is not default
                if (!string.IsNullOrEmpty(product.ProductImage) &&
                    product.ProductImage != "/images/default-product.jpg")
                {
                    DeleteProductImage(product.ProductImage);
                }

                product.ProductImage = await SaveProductImage(model.ProductImageFile);
            }

            _context.Entry(product).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Product updated successfully" });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = "Error updating product: " + ex.Message });
        }
    }

    [HttpPost("delete/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        try
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return Json(new { success = false, message = "Product not found" });
            }

            // Check if user owns this product
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (product.CreatorId != userId)
            {
                return Json(new { success = false, message = "Unauthorized to delete this product" });
            }

            // Delete image file if it exists and is not default
            if (!string.IsNullOrEmpty(product.ProductImage) &&
                product.ProductImage != "/images/default-product.jpg")
            {
                DeleteProductImage(product.ProductImage);
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Product deleted successfully" });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = "Error deleting product: " + ex.Message });
        }
    }

    [HttpGet("details/{id}")]
    public async Task<IActionResult> GetProductDetails(int id)
    {
        var product = await _context.Products
            .Include(p => p.Creator)
            .Include(p => p.Category)
            .Where(p => p.Id == id)
            .Select(p => new ProductViewModel
            {
                Id = p.Id,
                ProductName = p.ProductName,
                ProductImage = p.ProductImage,
                Price = p.Price,
                CreatorId = p.CreatorId,
                CreatorName = p.Creator.UserName ?? "Unknown",
                CategoryId = p.CategoryId,
                CategoryName = p.Category.CategoryName,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt
            })
            .FirstOrDefaultAsync();

        if (product == null)
        {
            return Json(new { success = false, message = "Product not found" });
        }

        return Json(new { success = true, data = product });
    }

    private async Task<string> SaveProductImage(IFormFile imageFile)
    {
        var uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "images", "products");

        if (!Directory.Exists(uploadsFolder))
        {
            Directory.CreateDirectory(uploadsFolder);
        }

        var uniqueFileName = Guid.NewGuid().ToString() + "_" + imageFile.FileName;
        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            await imageFile.CopyToAsync(fileStream);
        }

        return "/images/products/" + uniqueFileName;
    }

    private void DeleteProductImage(string imagePath)
    {
        if (string.IsNullOrEmpty(imagePath)) return;

        var physicalPath = Path.Combine(_hostEnvironment.WebRootPath, imagePath.TrimStart('/'));

        if (System.IO.File.Exists(physicalPath))
        {
            try
            {
                System.IO.File.Delete(physicalPath);
            }
            catch (Exception ex)
            {
                // Log error but don't throw
                Console.WriteLine($"Error deleting image: {ex.Message}");
            }
        }
    }
}