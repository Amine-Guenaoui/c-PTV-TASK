using Microsoft.AspNetCore.Mvc;
using NetTopologySuite.Geometries;
using StreetService.Core.Interfaces;
using StreetService.Core.Models;
using Microsoft.Extensions.Configuration;


namespace StreetService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StreetController : ControllerBase
{
    private readonly IStreetRepository _repository;
    private readonly IConfiguration _configuration;

    public StreetController(IConfiguration configuration, IStreetRepository repository)
    {
        _repository = repository;
         _configuration = configuration;  // Store the configuration
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

    [HttpPut("add-point/{id}")]
    public async Task<IActionResult> AddPoint(int id, [FromQuery] double x, [FromQuery] double y, [FromQuery] bool atStart = false)
    {
        var street = await _repository.GetByIdAsync(id);

        if (street == null)
        {
            return NotFound("Street not found");
        }

        // Handle feature flag to decide whether to use PostGIS or backend logic
        if (UseDatabasePostgis()) // Feature flag method to check for DB level operation
        {
            // Perform the operation using PostGIS (via EF Core or raw SQL)
            await AddPointToGeometryPostgis(street, x, y, atStart);
        }
        else
        {
            // Perform the operation algorithmically (in-memory, backend logic)
            await AddPointToGeometryBackend(street, x, y, atStart);
        }

        await _repository.UpdateAsync(street);
        return Ok(street);
    }

    private bool UseDatabasePostgis()
    {
        // Feature flag check - for now, just assume it's set in configuration
        return bool.Parse(_configuration["FeatureFlags:UsePostgis"]);
    }

    private async Task AddPointToGeometryPostgis(Street street, double x, double y, bool atStart)
    {
        // Create the new point to add
        var point = new Point(x, y) { SRID = 4326 };

        if (street.Geometry is LineString lineString)
        {
            // Add the point to the start (prepend) or end (append) of the line
            var coordinates = lineString.Coordinates.ToList(); // Convert to a list for easy manipulation

            if (atStart)
            {
                // Prepend the new point to the list
                coordinates.Insert(0, point.Coordinate);
            }
            else
            {
                // Append the new point to the list
                coordinates.Add(point.Coordinate);
            }

            // Create a new LineString with the updated coordinates
            street.Geometry = new LineString(coordinates.ToArray());
        }
        else if (street.Geometry == null)
        {
            // If the geometry is null, create a new LineString with just this point
            street.Geometry = new LineString(new[] { point.Coordinate });
        }
        else
        {
            // If it's not a LineString, return an error
            throw new InvalidOperationException("Street geometry is not a LineString.");
        }

        await _repository.UpdateAsync(street);  // Save the updated geometry
    }

    private async Task AddPointToGeometryBackend(Street street, double x, double y, bool atStart)
    {
        // Create the new point to add
        var point = new Point(x, y) { SRID = 4326 };

        if (street.Geometry is LineString lineString)
        {
            // Add the point to the start (prepend) or end (append) of the line
            var coordinates = lineString.Coordinates.ToList(); // Convert to a list for easy manipulation

            if (atStart)
            {
                // Prepend the new point to the list
                coordinates.Insert(0, point.Coordinate);
            }
            else
            {
                // Append the new point to the list
                coordinates.Add(point.Coordinate);
            }

            // Create a new LineString with the updated coordinates
            street.Geometry = new LineString(coordinates.ToArray());
        }
        else
        {
            // If the geometry is not a LineString, handle accordingly (e.g., throw an error)
            throw new InvalidOperationException("Street geometry is not a LineString.");
        }
    }



}
