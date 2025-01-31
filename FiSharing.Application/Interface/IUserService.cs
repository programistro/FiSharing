﻿using System.Collections;
using FiSharing.Core.Models;

namespace FiSharing.Application.Service;

public interface IUserService
{
    Task<User> GetByIdAsync(Guid uesrId);
    
    Task<IEnumerable<User>> GetAllAsync();
    
    Task AddAsync(User user);
    
    Task UpdateAsync(User user);
    
    Task DeleteAsync(Guid guid);
}