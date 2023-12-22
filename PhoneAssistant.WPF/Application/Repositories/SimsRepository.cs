using Microsoft.EntityFrameworkCore;

using PhoneAssistant.WPF.Application;
using PhoneAssistant.WPF.Application.Entities;

namespace PhoneAssistant.WPF.Application.Repositories;

public sealed class SimsRepository : ISimsRepository
{
    private readonly v1PhoneAssistantDbContext _dbContext;

    public SimsRepository(v1PhoneAssistantDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<v1Sim>> GetSimsAsync()
    {
        List<v1Sim> sims = await _dbContext.Sims.ToListAsync();
        return sims;
    }

    public async Task MoveSimToPhone(string phoneNumber, string imei)
    {
        if (phoneNumber is null)
        {
            throw new ArgumentNullException(nameof(phoneNumber));
        }

        v1Sim sim = await _dbContext.Sims.SingleAsync(x => x.PhoneNumber == phoneNumber);
        v1Phone phone = await _dbContext.Phones.SingleAsync(x => x.Imei == imei);

        phone.PhoneNumber = phoneNumber;
        phone.SimNumber = sim.SimNumber;
        _dbContext.Phones.Update(phone);

        _dbContext.Sims.Remove(sim);

        await _dbContext.SaveChangesAsync();
    }
}
