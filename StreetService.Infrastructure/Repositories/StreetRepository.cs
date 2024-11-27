using Microsoft.EntityFrameworkCore;
using StreetService.Core.Interfaces;
using StreetService.Core.Models;
using StreetService.Infrastructure.Data;

namespace StreetService.Infrastructure.Repositories;

public class StreetRepository : IStreetRepository
{
    private readonly AppDbContext _context;

    public StreetRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Street> GetByIdAsync(int id) => await _context.Streets.FindAsync(id);

    public async Task<List<Street>> GetAllAsync() => await _context.Streets.ToListAsync();

    public async Task AddAsync(Street street)
    {
        _context.Streets.Add(street);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Street street)
    {
        // _context.Streets.Update(street);
        // await _context.SaveChangesAsync();
        try
        {
            _context.Streets.Update(street);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            // Handle concurrency exception (if the version has changed in the database)
            throw new InvalidOperationException("Concurrency conflict occurred. The street has been updated by another user.");
        }
    }
    

    public async Task DeleteAsync(int id)
    {
        var street = await _context.Streets.FindAsync(id);
        if (street != null)
        {
            _context.Streets.Remove(street);
            await _context.SaveChangesAsync();
        }
    }
}
