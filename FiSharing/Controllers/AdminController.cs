using System.IO.Compression;
using System.Runtime.Intrinsics.X86;
using FiSharing.Application.Service;
using FiSharing.Core.Models;
using FiSharing.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FiSharing.Controllers;

[Authorize]
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
    
    [Authorize(Roles = "admin")]
    [HttpPost]
    public async Task<IActionResult> AddDepartament(DeportamentViewModel viewModel)
    {
        Directory.CreateDirectory($"files/{viewModel.Name}");
        
        List<string> files = new List<string>();

        if (viewModel.Files != null && viewModel.Files.Count > 0)
        {
            foreach (var formFile in viewModel.Files)
            {
                if (formFile.Length > 0)
                {
                    var fileName = formFile.FileName;

                    using (var stream = System.IO.File.Create($@"files/{viewModel.Name}/{fileName}"))
                    {
                        await formFile.CopyToAsync(stream);
                    }
                
                    files.Add(fileName);
                }
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
        
        return RedirectToAction("AdminPage", "Home");
    }
    
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> AddFileToDepartament(DeportamentViewModel viewModel, bool isAdmin = true)
    {
        var departament = await _departamentService.GetByNameAsync(viewModel.Name);

        if (departament != null)
        {
            long size = viewModel.Files.Sum(f => f.Length);

            List<string> files = new List<string>();
        
            foreach (var formFile in viewModel.Files)
            {
                if (formFile.Length > 0)
                {
                    var fileName = formFile.FileName;

                    using (var stream = System.IO.File.Create($@"files/{viewModel.Name}/{fileName}"))
                    {
                        await formFile.CopyToAsync(stream);
                    }
                
                    files.Add(fileName);
                }
            }
            
            foreach (var item in viewModel.Files)
            {
                departament.PathsToFiles.Add(item.FileName);
            }
            
            await _departamentService.UpdateAsync(departament);
        }

        if (isAdmin)
        {
            return RedirectToAction("AdminPage", "Home");
        }
        else
        {
            return RedirectToAction("Index", "Home");
        }
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> RemoveFileFromDepartament(DeportamentViewModel viewModel, bool isAdmin = true)
    {
        var departament = await _departamentService.GetByNameAsync(viewModel.Name);
        
        if (departament != null)
        {
            if (departament.PathsToFiles.Contains(viewModel.FileName))
            {
                departament.PathsToFiles.Remove(viewModel.FileName);
                await _departamentService.UpdateAsync(departament);

                if (System.IO.File.Exists($@"files/{viewModel.Name}/{viewModel.FileName}"))
                {
                    System.IO.File.Delete($@"files/{viewModel.Name}/{viewModel.FileName}");
                }
            }
        }

        if (isAdmin)
        {
            return RedirectToAction("AdminPage", "Home");
        }
        else
        {
            return RedirectToAction("Index", "Home");
        }
    }

    [Authorize(Roles = "admin")]
    [HttpPost]
    public async Task<IActionResult> RemoveDepartament(DeportamentViewModel viewModel)
    {
        var departament = await _departamentService.GetByNameAsync(viewModel.Name);

        if (departament != null)
        {
            await _departamentService.DeleteAsync(departament.Id);
        }
        
        return RedirectToAction("AdminPage", "Home");
    }
    
    [Authorize(Roles = "admin")]
    [HttpPost]
    public async Task<IActionResult> AddUserToDepartament(DeportamentViewModel viewModel)
    {
        var departament = await _departamentService.GetByNameAsync(viewModel.Name);

        var user = await _userService.GetByEmailAsync(viewModel.User);

        if (departament != null && user != null)
        {
            departament.Users.Add(user.Email);
            user.Departament = departament.Name;
            
            await _userService.UpdateAsync(user);
            await _departamentService.UpdateAsync(departament);
            
            return RedirectToAction("AdminPage", "Home");
        }
        else
        {   
            return RedirectToAction("AdminPage", "Home");
        }
    }
}