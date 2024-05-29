using Microsoft.EntityFrameworkCore;

using PhoneAssistant.WPF.Application.Entities;

namespace PhoneAssistant.WPF.Application.Repositories;

public sealed class DisposalsRepository : IDisposalsRepository
{
    private readonly PhoneAssistantDbContext _dbContext;

    public DisposalsRepository(PhoneAssistantDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(Disposal disposal)
    {
        _dbContext.Disposals.Add(disposal);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<Result> AddOrUpdateMSAsync(string imei, string status)
    {
        Disposal? disposal = await _dbContext.Disposals.FindAsync(imei);
        if (disposal is null)
        {
            disposal = new() { Imei = imei, StatusMS = status };
            await AddAsync(disposal);
            return Result.Added;
        }

        if (disposal.StatusMS == status)
            return Result.Unchanged;

        disposal.StatusMS = status;
        await UpdateAsync(disposal);
        return Result.Updated;
    }

    public async Task<Result> AddOrUpdatePAAsync(string imei, string status, int? sr)
    {
        Disposal? disposal = await _dbContext.Disposals.FindAsync(imei);
        if (disposal is null)
        {
            disposal = new() { Imei = imei, StatusPA = status, SR = sr };
            await AddAsync(disposal);
            return Result.Added;
        }

        if (disposal.StatusPA == status && disposal.SR == sr)
            return Result.Unchanged;

        disposal.StatusPA = status;
        disposal.SR = sr;

        await UpdateAsync(disposal);
        return Result.Updated;
    }

    public async Task<Result> AddOrUpdateSCCAsync(string imei, string status, int? certificate)
    {
        Disposal? disposal = await _dbContext.Disposals.FindAsync(imei);
        if (disposal is null)
        {
            disposal = new() { Imei = imei, StatusSCC = status, Certificate = certificate };
            await AddAsync(disposal);
            return Result.Added;
        }

        if (disposal.StatusSCC == status && disposal.Certificate == certificate)
            return Result.Unchanged;

        disposal.StatusSCC = status;
        disposal.Certificate = certificate;
        await UpdateAsync(disposal);
        return Result.Updated;
    }

    public async Task<IEnumerable<Disposal>> GetAllDisposalsAsync()
    {
        IEnumerable<Disposal> disposals = await _dbContext.Disposals.ToListAsync();
        return disposals;
    }

    public async Task<Disposal?> GetDisposalAsync(string imei)
    {
        Disposal? disposal = await _dbContext.Disposals.FindAsync(imei);
        return disposal;
    }

    public async Task TruncateAsync()
    {
        await _dbContext.Database.ExecuteSqlRawAsync("DELETE FROM ReconcileDisposals;");
        await _dbContext.Database.ExecuteSqlRawAsync("VACUUM;");
    }

    public async Task UpdateAsync(Disposal disposal)
    {
        _dbContext.Disposals.Update(disposal);
        await _dbContext.SaveChangesAsync();
    }
}