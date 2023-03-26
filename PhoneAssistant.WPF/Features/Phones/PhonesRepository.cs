using System.Collections.Generic;

using Microsoft.EntityFrameworkCore;

using PhoneAssistant.WPF.Application.Entities;
using PhoneAssistant.WPF.Features.Application;

namespace PhoneAssistant.WPF.Features.Phones;

public sealed class PhonesRepository : IPhonesRepository
{
    private readonly PhoneAssistantDbContext _dbContext;

    public PhonesRepository(PhoneAssistantDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<Phone>> GetPhonesAsync()
    {
        List<Phone> phones = await _dbContext.Phones.ToListAsync();
        return phones;
    }

    public async Task<IEnumerable<Phone>> SearchAsync(string search)
    {
        List<Phone> phones = await _dbContext.Phones
            .Where(p => p.Imei.Contains(search) || p.AssetTag.Contains(search))
            .ToListAsync();
        return phones;
    }

    public async Task UpdateAsync(Phone phoneToUpdate)
    {
        Phone phone = _dbContext.Phones.Where(mp => mp.Id == phoneToUpdate.Id).First();
        
        phone.Imei = phoneToUpdate.Imei;
        phone.FormerUser = phoneToUpdate.FormerUser;
        phone.Wiped = phoneToUpdate.Wiped;
        phone.Status = phoneToUpdate.Status;
        phone.OEM = phoneToUpdate.OEM;
        phone.AssetTag = phoneToUpdate.AssetTag;
        phone.Note = phoneToUpdate.Note;

        _dbContext.Phones.Update(phone);
        await _dbContext.SaveChangesAsync();
    }
}