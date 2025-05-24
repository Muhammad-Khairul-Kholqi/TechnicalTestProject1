using Microsoft.AspNetCore.Mvc;

public class AuthController : Controller
{
    public IActionResult Login()
    {
        return View();
    }

    [Route("/registration")]
    [HttpGet]
    public IActionResult Registration()
    {
        return View();
    }
}
