using Microsoft.AspNetCore.Mvc;

public class DashboardController : Controller
{
    [Route("/dashboard")]
    [HttpGet]
    public IActionResult IndexDashboard()
    {
        return View();
    }
}