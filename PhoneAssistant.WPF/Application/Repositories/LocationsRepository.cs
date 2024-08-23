using Microsoft.EntityFrameworkCore;

using PhoneAssistant.Model;
using PhoneAssistant.WPF.Application.Entities;

namespace PhoneAssistant.WPF.Application.Repositories;

public sealed class LocationsRepository : ILocationsRepository
{
    private readonly PhoneAssistantDbContext _dbContext;

    public LocationsRepository(PhoneAssistantDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<Location>> GetAllLocationsAsync()
    {
        IEnumerable<Location> locations = await _dbContext.Locations.ToListAsync();
        return locations;
    }
}
