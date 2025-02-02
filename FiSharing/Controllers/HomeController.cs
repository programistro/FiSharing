using System.Diagnostics;
using FiSharing.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using FiSharing.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace FiSharing.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    
    private readonly AppDbContext _context;

    public HomeController(ILogger<HomeController> logger, AppDbContext context)
    {
        _logger = logger;
        _context = context;
    }
    
    public IActionResult Index()
    {
        return View();
    }

    [Authorize(Roles = "admin")]
    public IActionResult Privacy()
    {
        return View();
    }

    [Authorize(Roles = "admin")]
    public IActionResult AdminPage()
    {
        return View();
    }

    [Authorize(Roles = "admin")]
    [HttpPost]
    public async Task<IActionResult> AddDepartament(DeportamentViewModel viewModel)
    {
        if (!ModelState.IsValid)
            return View("AdminPage", viewModel);
        
        long size = viewModel.Files.Sum(f => f.Length);

        foreach (var formFile in viewModel.Files)
        {
            if (formFile.Length > 0)
            {
                // var filePath = Path.GetTempFileName();
                var fileName = formFile.FileName;

                using (var stream = System.IO.File.Create($@"files/{fileName}"))
                {
                    await formFile.CopyToAsync(stream);
                }
            }
        }

        return View("AdminPage");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}