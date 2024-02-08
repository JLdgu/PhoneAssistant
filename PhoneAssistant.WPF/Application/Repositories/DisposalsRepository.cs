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

    public async Task<Disposal?> GetDisposalAsync(string imei)
    {
        Disposal? disposal = await _dbContext.Disposals.FirstOrDefaultAsync(d => d.Imei == imei);
        return disposal;
    }

    public async Task AddAsync(Disposal disposal)
    {
        await _dbContext.Disposals.AddAsync(disposal);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(Disposal disposal)
    {
        _dbContext.Disposals.Update(disposal);
        await _dbContext.SaveChangesAsync();
    }
}