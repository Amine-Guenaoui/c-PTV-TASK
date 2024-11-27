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

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Street street)
    {
        if (id != street.Id)
        {
            return BadRequest("Street ID mismatch.");  // Return 400 if IDs don't match
        }

        var existingStreet = await _repository.GetByIdAsync(id);
        if (existingStreet == null)
        {
            return NotFound();  // Return 404 if the street is not found
        }

        // Update the street (you may want to update more properties)
        existingStreet.Name = street.Name;
        existingStreet.Capacity = street.Capacity;
        existingStreet.Geometry = street.Geometry;

        await _repository.UpdateAsync(existingStreet);
        return Ok(existingStreet);  // Return the updated street
    }


    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var street = await _repository.GetByIdAsync(id);
        if (street == null)
        {
            return NotFound();  // Return 404 if the street is not found
        }

        await _repository.DeleteAsync(id);
        return NoContent();  // Return 204 (No Content) to indicate successful deletion
    }

}
