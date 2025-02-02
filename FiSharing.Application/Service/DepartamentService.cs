using System.Security.Cryptography;
using System.Text;
using FiSharing.Core.Models;
using FiSharing.Infrastructure.Repository;

namespace FiSharing.Application.Service;

public class DepartamentService : IDepartamentService
{
    private readonly IDepartamentRepository _departamentRepository;

    public DepartamentService(IDepartamentRepository departamentRepository)
    {
        _departamentRepository = departamentRepository;
    }
    
    public async Task<Department> GetByIdAsync(Guid uesrId)
    {
        return await _departamentRepository.GetByIdAsync(uesrId);
    }

    public async Task<IEnumerable<Department>> GetAllAsync()
    {
        return await _departamentRepository.GetAllAsync();
    }

    public async Task<Department> GetByNameAsync(string name)
    {
        return await _departamentRepository.GetByNameAsync(name);
    }

    public async Task AddAsync(Department user)
    {
        await _departamentRepository.AddAsync(user);
    }

    public async Task UpdateAsync(Department user)
    {
        await _departamentRepository.UpdateAsync(user);
    }

    public async Task DeleteAsync(Guid guid)
    {
        await _departamentRepository.DeleteAsync(guid);
    }

    public async Task<string> CreateHashPasswordAsync(string password)
    {
        using (SHA256 sha = SHA256.Create())
        {
            byte[] hashValue = sha.ComputeHash(Encoding.UTF8.GetBytes(password));

            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < hashValue.Length; i++)
            {
                builder.Append(hashValue[i].ToString("x2")); // Преобразуем байты хэша в шестнадцатеричное представление
            }

            return builder.ToString();
        }
    }
}