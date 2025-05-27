using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using technicalTestProject_1.Data;
using technicalTestProject_1.Models;
using System.Security.Claims;

[Route("/category")]
[Authorize]
public class CategoryController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<CategoryController> _logger;

    public CategoryController(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        ILogger<CategoryController> logger)
    {
        _context = context;
        _userManager = userManager;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> IndexCategory()
    {
        var user = await _userManager.GetUserAsync(User);
        ViewData["UserName"] = user?.FullName ?? user?.UserName ?? "User";
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var categories = await _context.Categories
                .Include(c => c.Creator)
                .Where(c => c.CreatorId == userId)
                .OrderByDescending(c => c.CreatedAt)
                .Select(c => new CategoryViewModel
                {
                    Id = c.Id,
                    CategoryName = c.CategoryName,
                    CreatorName = c.Creator.FullName ?? c.Creator.UserName,
                    CreatedAt = c.CreatedAt.ToString("dd MMMM yyyy")
                })
                .ToListAsync();

            ViewBag.Categories = categories;
            return View("~/Views/Category/IndexCategory.cshtml");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading categories");
            ViewBag.Categories = new List<CategoryViewModel>();
            return View("~/Views/Category/IndexCategory.cshtml");
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([FromBody] CreateCategoryViewModel model)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(x => x.Value?.Errors.Count > 0)
                    .SelectMany(x => x.Value!.Errors)
                    .Select(x => x.ErrorMessage);

                return Json(new { success = false, errors = errors });
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Check if category name already exists for this user
            var existingCategory = await _context.Categories
                .FirstOrDefaultAsync(c => c.CategoryName.ToLower() == model.CategoryName.ToLower()
                                    && c.CreatorId == userId);

            if (existingCategory != null)
            {
                return Json(new { success = false, message = "Category name already exists" });
            }

            var category = new Category
            {
                CategoryName = model.CategoryName.Trim(),
                CreatorId = userId!,
                CreatedAt = DateTime.UtcNow
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Category created: {CategoryName} by user {UserId}",
                category.CategoryName, userId);

            return Json(new
            {
                success = true,
                message = "Category created successfully!",
                category = new
                {
                    id = category.Id,
                    categoryName = category.CategoryName,
                    createdAt = category.CreatedAt.ToString("dd MMMM yyyy")
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating category");
            return Json(new { success = false, message = "An error occurred while creating the category" });
        }
    }

    [HttpPut]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update([FromBody] UpdateCategoryViewModel model)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(x => x.Value?.Errors.Count > 0)
                    .SelectMany(x => x.Value!.Errors)
                    .Select(x => x.ErrorMessage);

                return Json(new { success = false, errors = errors });
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == model.Id && c.CreatorId == userId);

            if (category == null)
            {
                return Json(new { success = false, message = "Category not found" });
            }

            // Check if new category name already exists for this user (excluding current category)
            var existingCategory = await _context.Categories
                .FirstOrDefaultAsync(c => c.CategoryName.ToLower() == model.CategoryName.ToLower()
                                    && c.CreatorId == userId && c.Id != model.Id);

            if (existingCategory != null)
            {
                return Json(new { success = false, message = "Category name already exists" });
            }

            category.CategoryName = model.CategoryName.Trim();
            category.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Category updated: {CategoryName} (ID: {CategoryId}) by user {UserId}",
                category.CategoryName, category.Id, userId);

            return Json(new
            {
                success = true,
                message = "Category updated successfully!",
                category = new
                {
                    id = category.Id,
                    categoryName = category.CategoryName
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating category with ID: {CategoryId}", model.Id);
            return Json(new { success = false, message = "An error occurred while updating the category" });
        }
    }

    [HttpDelete]
    [Route("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == id && c.CreatorId == userId);

            if (category == null)
            {
                return Json(new { success = false, message = "Category not found" });
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Category deleted: {CategoryName} (ID: {CategoryId}) by user {UserId}",
                category.CategoryName, category.Id, userId);

            return Json(new { success = true, message = "Category deleted successfully!" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting category with ID: {CategoryId}", id);
            return Json(new { success = false, message = "An error occurred while deleting the category" });
        }
    }

    [HttpGet]
    [Route("search")]
    public async Task<IActionResult> Search(string searchTerm = "", DateTime? date = null, int pageSize = 5)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var query = _context.Categories
                .Include(c => c.Creator)
                .Where(c => c.CreatorId == userId);

            // Search by category name
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(c => c.CategoryName.ToLower().Contains(searchTerm.ToLower()));
            }

            // Filter by date
            if (date.HasValue)
            {
                var searchDate = date.Value.Date;
                query = query.Where(c => c.CreatedAt.Date == searchDate);
            }

            var categories = await query
                .OrderByDescending(c => c.CreatedAt)
                .Take(pageSize)
                .Select(c => new CategoryViewModel
                {
                    Id = c.Id,
                    CategoryName = c.CategoryName,
                    CreatorName = c.Creator.FullName ?? c.Creator.UserName,
                    CreatedAt = c.CreatedAt.ToString("dd MMMM yyyy")
                })
                .ToListAsync();

            return Json(new { success = true, categories = categories });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching categories");
            return Json(new { success = false, message = "An error occurred while searching categories" });
        }
    }
}