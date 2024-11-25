using Microsoft.AspNetCore.Mvc;
using StreetService.Core.Interfaces;
using StreetService.Core.Models;

namespace StreetService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StreetController : ControllerBase
{
    private readonly IStreetRepository _repository;

    public StreetController(IStreetRepository repository)
    {
        _repository = repository;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id) => Ok(await _repository.GetByIdAsync(id));

    [HttpPost]
    public async Task<IActionResult> Create(Street street)
    {
        await _repository.AddAsync(street);
        return Ok(street);
    }
}
