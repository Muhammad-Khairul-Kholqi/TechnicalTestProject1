using Microsoft.AspNetCore.Mvc;

public class CategoryController : Controller
{
    [Route("/category")]
    [HttpGet]
    public IActionResult IndexCategory()
    {
        return View();
    }
}