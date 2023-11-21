using System.Xml;
using AutoMapper;
using CommandService.Data.Repository;
using CommandService.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace CommandService.Controllers;

[Route("api/c/[controller]")]
[ApiController]
public class PlatformsController : ControllerBase
{
    private readonly ICommandRepo _repository;
    private readonly IMapper _mapper;

    public PlatformsController(ICommandRepo repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PlatformReadDto>>> GetPlatforms()
    {
        Console.WriteLine("--> Getting platforms from command service");

        var platformItems = await _repository.GetAllPlatformsAsync();
        return Ok(_mapper.Map<IEnumerable<PlatformReadDto>>(platformItems));
    } 
    
    [HttpPost]
    public ActionResult TestInboundConnection()
    {
        Console.WriteLine("--> Inbound POST # Command Service");

        return Ok("Inbound POST # Command Service");
    }
}