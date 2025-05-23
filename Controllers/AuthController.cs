using Microsoft.AspNetCore.Mvc;

public class AuthController : Controller
{
    public IActionResult Login()
    {
        return View();
    }

    // mengatur rote untuk halaman registrasi (jika tidak menggunakan ini maka pathnya menjadi huruf besar)
    [Route("auth/registration")]
    [HttpGet]
    public IActionResult Registration()
    {
        return View();
    }
}
