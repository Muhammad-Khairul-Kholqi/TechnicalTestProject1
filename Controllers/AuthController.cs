using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using technicalTestProject_1.Models;
using System.Text.Json;
using System.Security.Claims;

namespace technicalTestProject_1.Controllers
{
    public class AuthController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<AuthController> _logger;
        private readonly IConfiguration _configuration;

        public AuthController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<AuthController> logger,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _configuration = configuration;
        }

        // [Route("/")]
        // [HttpGet]
        public IActionResult IndexLogin()
        {
            ViewData["GoogleClientId"] = _configuration["Authentication:Google:ClientId"];
            return View("~/Views/Auth/Login/IndexLogin.cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([FromBody] LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(
                    model.Email,
                    model.Password,
                    model.RememberMe,
                    lockoutOnFailure: false
                );

                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in.");
                    return Json(new { success = true, message = "Login successful!", redirectUrl = "/dashboard" });
                }

                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    return Json(new { success = false, message = "Account locked out." });
                }
                else
                {
                    return Json(new { success = false, message = "Invalid email or password." });
                }
            }

            var errors = ModelState
                .Where(x => x.Value?.Errors.Count > 0)
                .SelectMany(x => x.Value!.Errors)
                .Select(x => x.ErrorMessage);

            return Json(new { success = false, errors = errors });
        }

        [HttpPost]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleUserInfo googleUser)
        {
            var user = await _userManager.FindByEmailAsync(googleUser.Email);
            if (user == null)
            {
                return Json(new { success = false, message = "Akun Google belum terdaftar. Silakan registrasi terlebih dahulu." });
            }

            // Lakukan login
            await _signInManager.SignInAsync(user, isPersistent: false);
            return Json(new { success = true, redirectUrl = "/dashboard" });
        }

        [HttpGet]
        public async Task<IActionResult> GoogleResponse()
        {
            try
            {
                var info = await _signInManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    _logger.LogWarning("External login info not found");
                    return RedirectToAction("IndexLogin");
                }

                var result = await _signInManager.ExternalLoginSignInAsync(
                    info.LoginProvider,
                    info.ProviderKey,
                    isPersistent: false,
                    bypassTwoFactor: true);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in with {LoginProvider} provider.", info.LoginProvider);
                    return RedirectToAction("IndexDashboard", "Dashboard");
                }

                var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                var name = info.Principal.FindFirstValue(ClaimTypes.Name);

                if (string.IsNullOrEmpty(email))
                {
                    _logger.LogError("Email claim not received from Google");
                    return RedirectToAction("IndexLogin");
                }

                var user = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    FullName = name ?? email.Split('@')[0],
                    CreatedAt = DateTime.UtcNow,
                    EmailConfirmed = true
                };

                var creationResult = await _userManager.CreateAsync(user);
                if (!creationResult.Succeeded)
                {
                    _logger.LogError("User creation failed: {Errors}", string.Join(", ", creationResult.Errors));
                    return RedirectToAction("IndexLogin");
                }

                await _userManager.AddLoginAsync(user, info);
                await _signInManager.SignInAsync(user, isPersistent: false);

                _logger.LogInformation("New user created and signed in with Google");
                return RedirectToAction("IndexDashboard", "Dashboard");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GoogleResponse");
                return RedirectToAction("IndexLogin");
            }
        }

        [Route("registration")]
        [HttpGet]
        public IActionResult IndexRegistration()
        {
            ViewData["GoogleClientId"] = _configuration["Authentication:Google:ClientId"];
            return View("~/Views/Auth/Registration/IndexRegistration.cshtml", new RegisterViewModel());
        }

        [Route("/registration")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> IndexRegistration(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FullName = model.FullName ?? model.Email.Split('@')[0],
                    CreatedAt = DateTime.UtcNow
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    await _signInManager.SignInAsync(user, isPersistent: false);

                    return Json(new { success = true, message = "Registration successful!" });
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            var errors = ModelState
                .Where(x => x.Value?.Errors.Count > 0)
                .SelectMany(x => x.Value!.Errors)
                .Select(x => x.ErrorMessage);

            return Json(new { success = false, errors = errors });
        }

        [HttpPost]
        public async Task<IActionResult> GoogleRegister([FromBody] GoogleUserInfo googleUser)
        {
            var user = await _userManager.FindByEmailAsync(googleUser.Email);
            if (user != null)
            {
                return Json(new { success = false, message = "Akun Google sudah terdaftar. Silakan login." });
            }

            var newUser = new ApplicationUser
            {
                UserName = googleUser.Email,
                Email = googleUser.Email,
                // tambahkan field lain jika perlu
            };
            var result = await _userManager.CreateAsync(newUser);
            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(newUser, isPersistent: false);
                return Json(new { success = true, redirectUrl = "/dashboard" });
            }
            return Json(new { success = false, message = "Registrasi Google gagal." });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("IndexLogin", "Account");
        }

    }

    public class GoogleUserInfo
    {
        public string Email { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Picture { get; set; } = string.Empty;
        public string Sub { get; set; } = string.Empty; 
    }
}