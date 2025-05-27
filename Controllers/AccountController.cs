using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using technicalTestProject_1.Models;

namespace technicalTestProject_1.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(SignInManager<ApplicationUser> signInManager)
        {
            _signInManager = signInManager;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("IndexLogin", "Auth");
        }
    }
}
