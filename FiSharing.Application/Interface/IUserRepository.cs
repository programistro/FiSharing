using FiSharing.Core.Models;

namespace FiSharing.Application.Service;

public interface IUserRepository
{
    Task<User> GetByIdAsync(Guid userId);
    
    Task<User> GetByEmailAsync(string email);
    
    Task Add(User user);
    
    Task Update(User user);
    
    Task Delete(Guid userId);
    
    Task<IEnumerable<User>> GetAllAsync();
}