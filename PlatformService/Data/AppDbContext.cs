using Microsoft.EntityFrameworkCore;
using PlatformService.Models;

namespace PlatformService.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> opt) : base(opt)
    {
        Database.EnsureCreated();
    }
    
    public DbSet<Platform?> Platforms { get; set; }
}