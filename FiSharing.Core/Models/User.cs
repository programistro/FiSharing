namespace FiSharing.Core.Models;

public class User : BaseEntity
{
    public string PasswordHash { get; set; }
    
    public string Role { get; set; }
    
    public string Email { get; set; }
}