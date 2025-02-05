using FiSharing.Application.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FiSharing.Controllers;

[Authorize(Roles = "admin")]
public class AdminController : Controller
{
    private readonly ILogger<HomeController> _logger;
    
    private readonly IDepartamentService _departamentService;
    
    private readonly IUserService _userService;

    public AdminController(ILogger<HomeController> logger, IDepartamentService departamentService, IUserService userService)
    {
        _logger = logger;
        _departamentService = departamentService;
        _userService = userService;
    }
}