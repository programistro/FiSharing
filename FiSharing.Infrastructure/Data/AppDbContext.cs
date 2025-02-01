using FiSharing.Core.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure.Internal;

namespace FiSharing.Infrastructure.Data;

public class AppDbContext : IdentityDbContext<User>
{
    public DbSet<User> Users => Set<User>();

    public DbSet<Department> Departments => Set<Department>();
    
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    { }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=database.db");
    }
}