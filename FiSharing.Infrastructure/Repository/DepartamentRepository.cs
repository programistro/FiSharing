using FiSharing.Application.Service;
using FiSharing.Core.Models;
using FiSharing.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FiSharing.Infrastructure.Repository;

public class DepartamentRepository : IDepartamentRepository
{
    private readonly AppDbContext _context;

    public DepartamentRepository(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task<Department> GetByIdAsync(Guid departamentId)
    {
        var department = await _context.Departments.FirstOrDefaultAsync(x => x.Id == departamentId);
        
        if(department != null)
            return department;
        else
            return null;
    }

    public async Task<Department> GetByNameAsync(string name)
    {
        var department = await _context.Departments.FirstOrDefaultAsync(x => x.Name == name);
        
        if(department != null)
            return department;
        else
            return null;   
    }

    public async Task AddAsync(Department department)
    {
        await _context.Departments.AddAsync(department);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Department department)
    {
        _context.Departments.Update(department);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid departmentId)
    {
        var departament = await _context.Departments.FindAsync(departmentId);
        
        if (departament != null)
        {
            _context.Departments.Remove(departament);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<Department>> GetAllAsync()
    {
        return await _context.Departments.ToListAsync();
    }
}