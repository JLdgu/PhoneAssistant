using Microsoft.EntityFrameworkCore;
using PhoneAssistant.WPF.Application.Entities;

namespace PhoneAssistant.WPF.Application.Repositories;

public sealed class DisposalsRepository
{
    private readonly PhoneAssistantDbContext _dbContext;

    public DisposalsRepository(PhoneAssistantDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Disposal?> GetDisposal(string imei)
    {
        Disposal? disposal = await _dbContext.Disposals.FirstOrDefaultAsync(d => d.Imei == imei);
        return disposal;
    }

    public void Add(Disposal disposal)
    {
        _dbContext.Disposals.Add(disposal);
        //await _dbContext.SaveChangesAsync();
    }

    public async Task<bool> Save()
    {
        try
        {
            await _dbContext.SaveChangesAsync();
            return false;
        }
        catch (Exception ex) 
        { 
            return true; 
        }
    }

}