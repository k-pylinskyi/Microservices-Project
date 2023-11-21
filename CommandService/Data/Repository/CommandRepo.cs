using CommandService.Models;
using Microsoft.EntityFrameworkCore;

namespace CommandService.Data.Repository;

public class CommandRepo : ICommandRepo
{
    private readonly AppDbContext _context;

    public CommandRepo(AppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> SaveChangesAsync() =>
        await _context.SaveChangesAsync() >= 0;
    
    public async Task<IEnumerable<Platform>> GetAllPlatformsAsync() =>
        await _context.Platforms
            .ToListAsync();

    public async Task CreatePlatformAsync(Platform platform)
    {
        if (platform is null) throw new ArgumentNullException();
        await _context.Platforms.AddAsync(platform); 
    }
        

    public async Task<bool> PlatformExistsAsync(int platformId) => 
        await _context.Platforms
            .AnyAsync(p => p.Id == platformId);

    public async Task<bool> ExternalPlatformExistsAsync(int externalPlatformId)
    {
        return await _context.Platforms.AnyAsync(p => p.ExternalId == externalPlatformId);
    }

    public async Task<IEnumerable<Command>> GetCommandsForPlatformAsync(int platformId) =>
        await _context.Commands
            .Where(c => c.Platform.Id == platformId)
            .OrderBy(c => c.Platform.Name)
            .ToListAsync();

    public async Task<Command> GetCommandAsync(int platformId, int commandId) =>
        await _context.Commands
            .Where(c => c.Id == commandId && c.PlatformId == platformId)
            .FirstOrDefaultAsync();

    public async Task CreateCommandAsync(int platformId, Command command)
    {
        if (!await _context.Platforms.AnyAsync(p => p.Id == command.PlatformId)) throw new InvalidDataException();
        if (command is null) throw new ArgumentNullException();
        await _context.Commands.AddAsync(command);
    }
        
}