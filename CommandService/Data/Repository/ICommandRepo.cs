using CommandService.Models;

namespace CommandService.Data.Repository;

public interface ICommandRepo
{
    Task<bool> SaveChangesAsync();

    // Platforms
    Task<IEnumerable<Platform>> GetAllPlatformsAsync();
    Task CreatePlatformAsync(Platform platform);
    Task<bool> PlatformExistsAsync(int platformId);
    Task<bool> ExternalPlatformExistsAsync(int externalPlatformId);

    // Commands
    Task<IEnumerable<Command>> GetCommandsForPlatformAsync(int platformId);
    Task<Command> GetCommandAsync(int platformId, int commandId);
    Task CreateCommandAsync(int platformId, Command command);
}