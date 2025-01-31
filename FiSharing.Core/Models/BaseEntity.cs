using System.ComponentModel.DataAnnotations;

namespace FiSharing.Core.Models;

public class BaseEntity
{
    [Key]
    public Guid Id { get; set; }
}