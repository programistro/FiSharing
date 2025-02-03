using System.ComponentModel.DataAnnotations;

namespace FiSharing.Models;

public class DeportamentViewModel
{
    public List<IFormFile> Files { get; set; }
    
    public string Name { get; set; }
    
    public string Password { get; set; }
    
    public string? User { get; set; }
    
    public string? FileName { get; set; }
}