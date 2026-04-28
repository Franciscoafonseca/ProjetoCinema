using Microsoft.AspNetCore.Mvc;
using OnlineCinemaFestival.Api.Models;
using OnlineCinemaFestival.Api.Services;

namespace OnlineCinemaFestival.Api.Controllers;

[ApiController]
[Route("api/festivals")]
public class FestivalsController : ControllerBase
{
    private readonly FestivalService _service;

    public FestivalsController(FestivalService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<List<Festival>>> GetAll()
    {
        var festivals = await _service.GetAllAsync();
        return Ok(festivals);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Festival>> GetById(int id)
    {
        var festival = await _service.GetByIdAsync(id);

        if (festival == null)
            return NotFound();

        return Ok(festival);
    }

    [HttpPost]
    public async Task<ActionResult> Create(Festival festival)
    {
        try
        {
            await _service.AddAsync(festival);
            return CreatedAtAction(nameof(GetById), new { id = festival.Id }, festival);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
