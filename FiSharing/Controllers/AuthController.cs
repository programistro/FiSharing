using System.Security.Claims;
using FiSharing.Application.Service;
using FiSharing.Core.Models;
using FiSharing.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FiSharing.Controllers;

public class AuthController : Controller
{
    private readonly ILogger<AuthController> _logger;
    
    private readonly IUserService _userService;

    public AuthController(ILogger<AuthController> logger, IUserService userService, SignInManager<User> signInManager)
    {
        _logger = logger;
        _userService = userService;
    }

    public IActionResult RegisterPage()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(AuthViewModel viewModel)
    {
        if (ModelState.IsValid)
        {
            User user = new()
            {
                Email = viewModel.Email,
                Id = Guid.NewGuid().ToString(),
                Role = "user",
                PasswordHash = viewModel.Password
            };
            
            await _userService.AddAsync(user);
            
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.Role, user.Role),
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme); // Используем стандартную схему
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties
            {
                ExpiresUtc = DateTime.UtcNow.AddMinutes(120),
                IsPersistent = true
            });
            return RedirectToAction("Index", "Home");
        }
        
        return RedirectToAction("RegisterPage");
    }
}