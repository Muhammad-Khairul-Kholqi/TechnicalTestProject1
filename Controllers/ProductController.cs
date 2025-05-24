using Microsoft.AspNetCore.Mvc;

public class ProductController : Controller
{
    [Route("/products")]
    [HttpGet]
    public IActionResult IndexProduct()
    {
        return View();
    }
}