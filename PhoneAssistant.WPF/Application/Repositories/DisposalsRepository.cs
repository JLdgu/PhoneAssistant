using PhoneAssistant.WPF.Application.Entities;

namespace PhoneAssistant.WPF.Application.Repositories;

public sealed class DisposalsRepository : IDisposalsRepository
{
    private readonly PhoneAssistantDbContext _dbContext;

    public DisposalsRepository(PhoneAssistantDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Disposal?> GetDisposalAsync(string imei)
    {
        Disposal? disposal = await _dbContext.Disposals.FindAsync(imei);
        return disposal;
    }

    public async Task AddAsync(Disposal disposal)
    {
        _dbContext.Disposals.Add(disposal);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<Result> AddOrUpdateAsync(Import import, Disposal disposal)
    {
        Disposal? existing = await _dbContext.Disposals.FindAsync(disposal.Imei);
        if (existing is null)
        {
            _dbContext.Disposals.Add(disposal);
            await _dbContext.SaveChangesAsync();
            return Result.Added;
        }

        switch (import)
        {
            case Import.DCC:
                if (existing.StatusDCC == disposal.StatusDCC)
                    return Result.Unchanged;
                else
                    existing.StatusDCC = disposal.StatusDCC;
                break;
            case Import.PA:
                if (existing.StatusPA == disposal.StatusPA)
                    return Result.Unchanged;
                else
                    existing.StatusPA = disposal.StatusPA;
                break;
            case Import.SCC:
                if (existing.StatusSCC == disposal.StatusSCC)
                    return Result.Unchanged;
                else
                    existing.StatusSCC = disposal.StatusSCC;
                break;
        }


        _dbContext.Disposals.Update(existing);
        await _dbContext.SaveChangesAsync();
        return Result.Updated;
    }

    public async Task UpdateAsync(Disposal disposal)
    {
        _dbContext.Disposals.Update(disposal);
        await _dbContext.SaveChangesAsync();
    }
}