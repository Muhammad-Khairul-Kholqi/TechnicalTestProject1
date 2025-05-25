using Microsoft.AspNetCore.Mvc;

public class UserController : Controller
{
    [Route("/user")]
    [HttpGet]
    public IActionResult IndexUser()
    {
        return View();
    }
}