using Microsoft.EntityFrameworkCore;

using PhoneAssistant.WPF.Application;
using PhoneAssistant.WPF.Application.Entities;

namespace PhoneAssistant.WPF.Application.Repositories;

public sealed class SimsRepository : ISimsRepository
{
    private readonly PhoneAssistantDbContext _dbContext;

    public SimsRepository(PhoneAssistantDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<string?> DeleteSIMAsync(string phoneNumber)
    {
        Sim? sim = await _dbContext.Sims.FindAsync(phoneNumber);
        
        if (sim is null) return null;

        _dbContext.Sims.Remove(sim);
        await _dbContext.SaveChangesAsync();
        return sim.SimNumber;
    }

    public Task<Sim?> GetSIMAsync(string phoneNumber)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<Sim>> GetSimsAsync()
    {
        List<Sim> sims = await _dbContext.Sims.ToListAsync();
        return sims;
    }

    //public async Task MoveSimToPhone(string phoneNumber, string imei)
    //{
    //    if (phoneNumber is null)
    //    {
    //        throw new ArgumentNullException(nameof(phoneNumber));
    //    }

    //    Sim sim = await _dbContext.Sims.SingleAsync(x => x.PhoneNumber == phoneNumber);
    //    Phone phone = await _dbContext.Phones.SingleAsync(x => x.Imei == imei);

    //    phone.PhoneNumber = phoneNumber;
    //    phone.SimNumber = sim.SimNumber;
    //    _dbContext.Phones.Update(phone);

    //    _dbContext.Sims.Remove(sim);

    //    await _dbContext.SaveChangesAsync();
    //}
}
