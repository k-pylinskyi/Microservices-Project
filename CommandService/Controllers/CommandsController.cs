using AutoMapper;
using CommandService.Data.Repository;
using CommandService.Dtos;
using CommandService.Models;
using Microsoft.AspNetCore.Mvc;

namespace CommandService.Controllers;

[Route("api/platforms/{platformId}/[controller]")]
[ApiController]
public class CommandsController : ControllerBase
{
    private readonly ICommandRepo _repository;
    private readonly IMapper _mapper;

    public CommandsController(ICommandRepo repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CommandReadDto>>> GetCommandsPlatform(int platformId)
    {
        Console.WriteLine($"--> Hit Get Commands For Platform {platformId}");

        if (!await _repository.PlatformExistsAsync(platformId)) return NotFound();

        var commands = await _repository.GetCommandsForPlatformAsync(platformId);
        return Ok(_mapper.Map<IEnumerable<CommandReadDto>>(commands));
    }

    [HttpGet("{commandId}", Name = nameof(GetCommandForPlatform))]
    public async Task<ActionResult<CommandReadDto>> GetCommandForPlatform(int platformId, int commandId)
    {
        Console.WriteLine($"--> Hit Get Command {commandId} For Platform {platformId}");

        if (!await _repository.PlatformExistsAsync(platformId)) return NotFound();

        var command = await _repository.GetCommandAsync(platformId, commandId);
        return Ok(_mapper.Map<CommandReadDto>(command));
    }

    [HttpPost]
    public async Task<ActionResult<CommandReadDto>> CreateCommandForPlatform(int platformId, [FromForm] CommandCreateDto commandCreateDto)
    {
        Console.WriteLine($"--> Hit Create Command For Platform {platformId}");

        if (!await _repository.PlatformExistsAsync(platformId)) return NotFound();

        var command = _mapper.Map<Command>(commandCreateDto);
        await _repository.CreateCommandAsync(platformId, command);
        await _repository.SaveChangesAsync();

        var commandReadDto = _mapper.Map<CommandReadDto>(command);
        return CreatedAtRoute(nameof(GetCommandForPlatform),
            new { platformId = platformId, commandId = commandReadDto.Id }, commandReadDto);
    }
    
}