using FiSharing.Core.Models;

namespace FiSharing.Application.Service;

public interface IDepartamentRepository
{
    Task<Department> GetByIdAsync(Guid departmentId);
    
    Task<Department> GetByNameAsync(string name);
    
    Task AddAsync(Department department);
    
    Task UpdateAsync(Department department);
    
    Task DeleteAsync(Guid departmentId);
    
    Task<IEnumerable<Department>> GetAllAsync();
}