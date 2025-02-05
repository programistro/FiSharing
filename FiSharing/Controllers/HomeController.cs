using System.Diagnostics;
using System.IO.Compression;
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
    
    public HomeController(ILogger<HomeController> logger, IDepartamentService departamentService)
    {
        _logger = logger;
        _departamentService = departamentService;
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

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> DownloadFileDeportament(DeportamentViewModel viewModel)
    {
        var departament = await _departamentService.GetByNameAsync(viewModel.Name);

        if (departament != null)
        {
            var zipName = $"файлы - {departament.Name}.zip";  
            using (MemoryStream ms = new MemoryStream())  
            {  
                using (var zip = new ZipArchive(ms, ZipArchiveMode.Create, true))  
                {  
                    //QUery the Products table and get all image content  
                    viewModel.SelectedFiles.ToList().ForEach(file =>  
                    {  
                        var entry = zip.CreateEntry(file);
                        
                        using (var fileStream = new MemoryStream(System.IO.File.ReadAllBytes($"files/{file}")))  
                        using (var entryStream = entry.Open())  
                        {  
                            fileStream.CopyTo(entryStream);  
                        }  
                    });  
                }   
                return File( ms.ToArray(), "application/zip", zipName);   
            }
        }

        return View("Index");
    }
    
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}