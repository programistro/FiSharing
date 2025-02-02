namespace FiSharing.Core.Models;

public class Department : BaseEntity
{
    public string Name { get; set; }
    
    public string PasswordHash { get; set; }
    
    public List<string> PathsToFiles { get; set; } = new();
    
    public List<string> Users { get; set; } = new();
}