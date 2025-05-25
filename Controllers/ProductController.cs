using Microsoft.AspNetCore.Mvc;

public class ProductController : Controller
{
    [Route("/product")]
    [HttpGet]
    public IActionResult IndexProduct()
    {
        return View();
    }
}