using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using technicalTestProject_1.Models;
using System.Threading.Tasks;
using System.IO;
// Add the correct namespace for ApplicationUser if different
using technicalTestProject_1.Models; // Ensure ApplicationUser is in this namespace, or update accordingly
// using Nawatech.YourNamespace.Models; // Uncomment and update if needed

[Route("/profile")]
[Authorize]
public class ProfileController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IWebHostEnvironment _env;

    public ProfileController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IWebHostEnvironment env)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _env = env;
    }

    [HttpGet]
    public async Task<IActionResult> IndexProfile()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return NotFound();

        var model = new UpdateProfileViewModel
        {
            FullName = user.FullName,
            UserName = user.UserName,
            Email = user.Email,
            // ProfileImageUrl = string.IsNullOrEmpty(user.ProfileImage) ? null : $"/uploads/profile/{user.ProfileImage}"
        };

        return View(model);
    }

    [HttpPost("update")]
    public async Task<IActionResult> UpdateProfile(UpdateProfileViewModel model)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return NotFound();

        user.FullName = model.FullName;
        user.UserName = model.UserName;
        user.Email = model.Email;

        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);
        }

        return RedirectToAction("IndexProfile");
    }

    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword(string oldPassword, string newPassword, string confirmPassword)
    {
        if (newPassword != confirmPassword)
        {
            ModelState.AddModelError("", "New password and confirmation do not match.");
            return RedirectToAction("IndexProfile");
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null) return NotFound();

        var result = await _userManager.ChangePasswordAsync(user, oldPassword, newPassword);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);
        }
        else
        {
            await _signInManager.RefreshSignInAsync(user);
        }

        return RedirectToAction("IndexProfile");
    }

    [HttpPost("update-image")]
    public async Task<IActionResult> UpdateProfileImage(IFormFile profileImage)
    {
        if (profileImage == null || profileImage.Length == 0)
            return RedirectToAction("IndexProfile");

        var user = await _userManager.GetUserAsync(User);
        if (user == null) return NotFound();

        // Create folder if not exists
        var uploadPath = Path.Combine(_env.WebRootPath, "uploads", "profile");
        if (!Directory.Exists(uploadPath))
            Directory.CreateDirectory(uploadPath);

        // Save the image
        var fileName = $"{user.Id}_{Path.GetFileName(profileImage.FileName)}";
        var filePath = Path.Combine(uploadPath, fileName);
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await profileImage.CopyToAsync(stream);
        }

        // Update user profile
        // user.ProfileImage = fileName;
        await _userManager.UpdateAsync(user);

        return RedirectToAction("IndexProfile");
    }
}
