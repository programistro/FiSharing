using System.Security.Cryptography;
using System.Text;
using FiSharing.Core.Models;
using FiSharing.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FiSharing.Application.Service;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    
    public async Task<User> GetByIdAsync(Guid userId)
    {
        return await _userRepository.GetByIdAsync(userId);
    }

    public async Task<User> GetByEmailAsync(string email)
    {
        return await _userRepository.GetByEmailAsync(email);
    }

    public async Task AddAsync(User user)
    {
        using (SHA256 sha = SHA256.Create())
        {
            byte[] hashValue = sha.ComputeHash(Encoding.UTF8.GetBytes(user.PasswordHash));

            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < hashValue.Length; i++)
            {
                builder.Append(hashValue[i].ToString("x2")); // Преобразуем байты хэша в шестнадцатеричное представление
            }

            user.PasswordHash = builder.ToString();
        }
        
        await _userRepository.Add(user);
    }

    public async Task UpdateAsync(User user)
    {
        await _userRepository.Update(user);
    }

    public async Task DeleteAsync(Guid userId)
    {
        await _userRepository.Delete(userId);
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        return await _userRepository.GetAllAsync();
    }
}