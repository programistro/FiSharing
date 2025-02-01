using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace FiSharing.Core.Models;

public class User : IdentityUser
{
    public string Role { get; set; }
}