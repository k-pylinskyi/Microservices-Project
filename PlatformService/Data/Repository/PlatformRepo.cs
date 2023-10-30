using Microsoft.EntityFrameworkCore;
using PlatformService.Models;

namespace PlatformService.Data.Repository;

public class PlatformRepo : IPlatformRepo
{
    private readonly AppDbContext _context;

    public PlatformRepo(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task<bool> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync() >= 0;
    }

    public async Task<IEnumerable<Platform?>> GetAllPlatformsAsync()
    {
        return await _context.Platforms.ToListAsync();
    }

    public async Task<Platform?> GetPlatformByIdAsync(int id)
    {
        return await _context.Platforms.FirstOrDefaultAsync(p => p != null && p.Id == id);
    }

    public async Task CreatePlatformAsync(Platform? platform)
    {
        if (platform == null)
        {
            throw new ArgumentNullException(nameof(platform));
        }

        await _context.Platforms.AddAsync(platform);
    }
}