using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.Data.Repository;
using PlatformService.Dtos;
using PlatformService.Models;
using PlatformService.SyncDataService.Http;

namespace PlatformService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PlatformsController : ControllerBase
{
    private readonly IPlatformRepo _repository;
    private readonly IMapper _mapper;
    private readonly ICommandDataClient _commandDataClinet;

    public PlatformsController(IPlatformRepo repository, IMapper mapper, ICommandDataClient commandDataClient)
    {
        _repository = repository;
        _mapper = mapper;
        _commandDataClinet = commandDataClient;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PlatformReadDto>>> GetPlatforms()
    {
        var platformItems = await _repository.GetAllPlatformsAsync();

        return Ok(_mapper.Map<IEnumerable<PlatformReadDto>>(platformItems));
    }
    
    [HttpGet("{id}", Name = nameof(GetPlatformById))]
    public async Task<ActionResult<PlatformReadDto>> GetPlatformById(int id)
    {
        var platform = await _repository.GetPlatformByIdAsync(id);

        if (platform == null) return NotFound();
        
        return Ok(_mapper.Map<PlatformReadDto>(platform));
    }

    [HttpPost]
    public async Task<ActionResult<PlatformReadDto>> CreatePlatform(PlatformCreateDto platform)
    {
        var platformModel = _mapper.Map<Platform>(platform);
        await _repository.CreatePlatformAsync(platformModel);
        await _repository.SaveChangesAsync();

        var platformReadDto = _mapper.Map<PlatformReadDto>(platformModel);

        try
        {
            await _commandDataClinet.SendPlatformToCommand(platformReadDto);
        }
        catch(Exception ex)
        {
            Console.WriteLine($"--> Could not send synchronously: {ex}");
        }

        return CreatedAtRoute(nameof(GetPlatformById), new { Id = platformReadDto.Id }, platformReadDto);
    }
}