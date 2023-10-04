using Microsoft.EntityFrameworkCore;

using PhoneAssistant.WPF.Application;
using PhoneAssistant.WPF.Application.Entities;

namespace PhoneAssistant.WPF.Features.Phones;

public sealed class PhonesRepository : IPhonesRepository
{
    private readonly v1PhoneAssistantDbContext _dbContext;

    public PhonesRepository(v1PhoneAssistantDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<v1Phone>> GetPhonesAsync()
    {
        List<v1Phone> phones = await _dbContext.Phones
                               .AsNoTracking()
                               .OrderByDescending(p => p.LastUpdate)
                               .ToListAsync();
        return phones;
    }

    public async Task RemoveSimFromPhone(v1Phone phone)
    {
        if (phone is null)
        {
            throw new ArgumentNullException(nameof(phone));
        }
        if (string.IsNullOrEmpty(phone.PhoneNumber))
        {
            throw new ArgumentException($"'{nameof(phone.PhoneNumber)}' cannot be null or empty.", nameof(phone.PhoneNumber));
        }
        if (string.IsNullOrEmpty(phone.SimNumber))
        {
            throw new ArgumentException($"'{nameof(phone.SimNumber)}' cannot be null or empty.", nameof(phone.SimNumber));
        }

        v1Phone dbPhone = await _dbContext.Phones.SingleAsync(x => x.Imei == phone.Imei);
        v1Sim? sim = await _dbContext.Sims.FindAsync(phone.PhoneNumber);
        if (sim is not null)
        {
            throw new InvalidOperationException($"'{nameof(phone.PhoneNumber)}' already exists.");
        }
        sim = new()
        {
            PhoneNumber = phone.PhoneNumber,
            SimNumber = phone.SimNumber,
            Status = "In Stock"
        };
        _dbContext.Sims.Add(sim);
        await _dbContext.SaveChangesAsync();
    }
    //public async Task<IEnumerable<v1Phone>> SearchAsync(string search)
    //{
    //    List<v1Phone> phones = await _dbContext.Phones
    //        .Where(p => p.Imei.Contains(search) || p.AssetTag.Contains(search))
    //        .ToListAsync();
    //    return phones;
    //}

    //public async Task UpdateAsync(Phone phoneToUpdate)
    //{
    //    Phone phone = _dbContext.Phones.Where(mp => mp.Id == phoneToUpdate.Id).First();

    //    phone.Imei = phoneToUpdate.Imei;
    //    phone.FormerUser = phoneToUpdate.FormerUser;
    //    phone.Wiped = phoneToUpdate.Wiped;
    //    phone.Status = phoneToUpdate.Status;
    //    phone.OEM = phoneToUpdate.OEM;
    //    phone.AssetTag = phoneToUpdate.AssetTag;
    //    phone.Note = phoneToUpdate.Note;

    //    _dbContext.Phones.Update(phone);
    //    await _dbContext.SaveChangesAsync();
    //}
}