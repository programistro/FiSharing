namespace FiSharing.Core.Models;

public class Department : BaseEntity
{
    public string Name { get; set; }
    
    public string PasswordHash { get; set; }
}