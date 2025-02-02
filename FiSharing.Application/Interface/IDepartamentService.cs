using FiSharing.Core.Models;

namespace FiSharing.Application.Service;

public interface IDepartamentService
{
    Task<Department> GetByIdAsync(Guid uesrId);
    
    Task<IEnumerable<Department>> GetAllAsync();
    
    Task<Department> GetByNameAsync(string name);
    
    Task AddAsync(Department user);
    
    Task UpdateAsync(Department user);
    
    Task DeleteAsync(Guid guid);
    
    Task<string> CreateHashPasswordAsync(string password);
}