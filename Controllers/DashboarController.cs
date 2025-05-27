using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using technicalTestProject_1.Data; // Add this line or replace with the correct namespace for ApplicationDbContext

namespace technicalTestProject_1.Controllers
{
    [Route("/dashboard")]
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [HttpGet]
        public async Task<IActionResult> IndexDashboard()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var totalProduct = await _context.Products
                .Where(p => p.CreatorId == userId)
                .CountAsync();

            var totalCategory = await _context.Categories
                .Where(c => c.CreatorId == userId)
                .CountAsync();

            // Group product by day of week
            var productData = await _context.Products
                .Where(p => p.CreatorId == userId)
                .GroupBy(p => p.CreatedAt.DayOfWeek)
                .Select(g => new {
                    Day = g.Key.ToString(),
                    Count = g.Count()
                })
                .ToListAsync();

            // Group category by day of week
            var categoryData = await _context.Categories
                .Where(c => c.CreatorId == userId)
                .GroupBy(c => c.CreatedAt.DayOfWeek)
                .Select(g => new {
                    Day = g.Key.ToString(),
                    Count = g.Count()
                })
                .ToListAsync();

            // Normalize order of days (Sunday to Saturday)
            var days = Enum.GetNames(typeof(DayOfWeek));
            var productCounts = days.Select(day =>
                productData.FirstOrDefault(p => p.Day == day)?.Count ?? 0).ToList();

            var categoryCounts = days.Select(day =>
                categoryData.FirstOrDefault(c => c.Day == day)?.Count ?? 0).ToList();

            ViewBag.TotalProduct = totalProduct;
            ViewBag.TotalCategory = totalCategory;
            ViewBag.ProductChartData = System.Text.Json.JsonSerializer.Serialize(productCounts);
            ViewBag.CategoryChartData = System.Text.Json.JsonSerializer.Serialize(categoryCounts);

            return View("~/Views/Dashboard/IndexDashboard.cshtml");
        }


    }
}