using System.Diagnostics;
using FiSharing.Application.Service;
using FiSharing.Core.Models;
using FiSharing.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using FiSharing.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace FiSharing.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    
    private readonly IDepartamentService _departamentService;
    
    private readonly IUserService _userService;

    public HomeController(ILogger<HomeController> logger, IDepartamentService departamentService, IUserService userService)
    {
        _logger = logger;
        _departamentService = departamentService;
        _userService = userService;
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

        List<string> files = new List<string>();
        
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
                
                files.Add(fileName);
            }
        }
        
        Department department = new()
        {
            Name = viewModel.Name,
            Id = Guid.NewGuid(),
            PasswordHash = viewModel.Password,
            PathsToFiles = files,
            Users = new()
            {
                viewModel.User
            }
        };
        
        await _departamentService.AddAsync(department);
        
        return View("AdminPage");
    }

    [Authorize(Roles = "admin")]
    [HttpPost]
    public async Task<IActionResult> AddFileToDepartament(DeportamentViewModel viewModel)
    {
        var departament = await _departamentService.GetByNameAsync(viewModel.Name);

        if (departament != null)
        {
            foreach (var item in viewModel.Files)
            {
                departament.PathsToFiles.Add(item.FileName);
            }
            
            await _departamentService.UpdateAsync(departament);
        }
        
        return View("AdminPage");
    }

    [Authorize(Roles = "admin")]
    [HttpPost]
    public async Task<IActionResult> RemoveFileFromDepartament(DeportamentViewModel viewModel)
    {
        var departament = await _departamentService.GetByNameAsync(viewModel.Name);
        
        if (departament != null)
        {
            if (departament.PathsToFiles.Contains(viewModel.FileName))
            {
                departament.PathsToFiles.Remove(viewModel.FileName);
                await _departamentService.UpdateAsync(departament);
            }
            else
            {
                return View("AdminPage");        
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