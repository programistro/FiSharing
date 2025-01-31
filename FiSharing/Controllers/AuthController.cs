using FiSharing.Models;
using Microsoft.AspNetCore.Mvc;

namespace FiSharing.Controllers;

public class AuthController : Controller
{
    private readonly ILogger<AuthController> _logger;

    public AuthController(ILogger<AuthController> logger)
    {
        _logger = logger;
    }

    public IActionResult RegisterPage()
    {
        return View();
    }

    public IActionResult Register(AuthViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            
            
            return View("_Layout");
        }
        
        return View("_Layout");
    }
}