using System.Text.Json;
using AutoMapper;
using CommandService.Data.Repository;
using CommandService.Dtos;
using CommandService.Models;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace CommandService.EventProcessing;

public class EventProcessor : IEventProcessor 
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IMapper _mapper;

    public EventProcessor(IServiceScopeFactory scopeFactory, IMapper mapper)
    {
        _scopeFactory = scopeFactory;
        _mapper = mapper;
    }
    
    public async Task ProcessEvent(string message)
    {
        var eventType = DetermineEvent(message);

        switch (eventType)
        {
            case EventType.PlatformPublished:
                await AddPlatform(message);
                break;
            
            default:
                break;
        }
    }

    private EventType DetermineEvent(string notificationMessage)
    {
        Console.WriteLine("--> Determine Event");

        var eventType = JsonSerializer.Deserialize<GenericEventDto>(notificationMessage);

        switch (eventType.Event)
        {
            case "Platform_Published":
                Console.WriteLine("--> Platform Published Event detected");
                return EventType.PlatformPublished;
            
            default:
                Console.WriteLine("--> Undetermined Event detected");
                return EventType.Undetermined;
        }
    }

    private async Task AddPlatform(string platformPublishedMessage)
    {
        using (var scope = _scopeFactory.CreateScope())
        {
            var repo = scope.ServiceProvider.GetRequiredService<ICommandRepo>();

            var platformPublishedDto = JsonSerializer.Deserialize<PlatformPublishedDto>(platformPublishedMessage);
            try
            {
                var platform = _mapper.Map<Platform>(platformPublishedDto);
                if (!await repo.ExternalPlatformExistsAsync(platform.ExternalId))
                {
                    await repo.CreatePlatformAsync(platform);
                    await repo.SaveChangesAsync();
                    
                    Console.WriteLine("--> Platform added");
                }
                else
                {
                    Console.WriteLine("--> Platform already exists");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Could not add Platform to DB {ex.Message}");
            }
        }
    }
}

enum EventType
{
    PlatformPublished,
    Undetermined
}