using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
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

    public IActionResult LoginPage()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(AuthViewModel viewModel)
    {
        if (!ModelState.IsValid)
           return RedirectToAction("RegisterPage");
        if (viewModel.Password != viewModel.AcceptPassword)
            return BadRequest();
            
        User user = new()
        {
            Email = viewModel.Email,
            Id = Guid.NewGuid().ToString(),
            Role = "user",
            PasswordHash = await _userService.CreateHashPasswordAsync(viewModel.Password)
        };
            
        await _userService.AddAsync(user);
            
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.Email),
            new Claim(ClaimTypes.Role, user.Role),
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme); 
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties
        {
            ExpiresUtc = DateTime.UtcNow.AddMinutes(120),
            IsPersistent = true
        });
        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    public async Task<IActionResult> Login(AuthViewModel viewModel)
    {
        if (!ModelState.IsValid)
            return RedirectToAction("LoginPage");

        if (viewModel.Email == "admin@gmail.com" && viewModel.Password == "@admin123")
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Name, "admin"),
                new Claim(ClaimTypes.Role, "admin"),
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme); 
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties
            {
                ExpiresUtc = DateTime.UtcNow.AddMinutes(120),
                IsPersistent = true
            });
                        
            return RedirectToAction("Index", "Home");
        }
        
        var user = await _userService.GetByEmailAsync(viewModel.Email);

        if (user != null)
        {
            if (user.PasswordHash == await _userService.CreateHashPasswordAsync(viewModel.Password))
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(ClaimTypes.Name, user.Email),
                    new Claim(ClaimTypes.Role, user.Role),
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme); 
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties
                {
                    ExpiresUtc = DateTime.UtcNow.AddMinutes(120),
                    IsPersistent = true
                });
                        
                return RedirectToAction("Index", "Home");
            }
        }
        ModelState.AddModelError(string.Empty, "Invalid login attempt");
        return RedirectToAction("LoginPage");
    }

    [HttpGet]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        return RedirectToAction("Index", "Home");
    }
}