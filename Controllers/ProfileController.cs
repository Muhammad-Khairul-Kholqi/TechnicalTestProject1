using Microsoft.AspNetCore.Mvc;

public class ProfileController : Controller
{
    [Route("/profile")]
    [HttpGet]
    public IActionResult IndexProfile()
    {
        return View();
    }
}