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
        if (ModelState.IsValid)
        {
            if (viewModel.Password != viewModel.AcceptPassword)
            {
                return BadRequest();
            }
            
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

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme); 
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

    [HttpPost]
    public async Task<IActionResult> Login(AuthViewModel viewModel)
    {
        if (ModelState.IsValid)
        {
            var user = await _userService.GetByEmailAsync(viewModel.Email);

            if (user != null)
            {
                using (SHA256 sha = SHA256.Create())
                {
                    byte[] hashValue = sha.ComputeHash(Encoding.UTF8.GetBytes(viewModel.Password));

                    StringBuilder builder = new StringBuilder();
                    for (int i = 0; i < hashValue.Length; i++)
                    {
                        builder.Append(hashValue[i].ToString("x2")); // Преобразуем байты хэша в шестнадцатеричное представление
                    }

                    if (user.PasswordHash == builder.ToString())
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
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Invalid login attempt");
                    }
                }
            }
        }
        return RedirectToAction("LoginPage");
    }

    [HttpGet]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        return RedirectToAction("Index", "Home");
    }
}