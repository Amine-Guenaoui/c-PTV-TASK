namespace StreetService.Core.Interfaces;

using StreetService.Core.Models;

public interface IStreetRepository
{
    Task<Street> GetByIdAsync(int id);
    Task<List<Street>> GetAllAsync();
    Task AddAsync(Street street);
    Task UpdateAsync(Street street);
    Task DeleteAsync(int id);
}
