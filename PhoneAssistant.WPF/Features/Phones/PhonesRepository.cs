using Microsoft.EntityFrameworkCore;

using PhoneAssistant.WPF.Application.Entities;
using PhoneAssistant.WPF.Features.Application;
using PhoneAssistant.WPF.Models;

namespace PhoneAssistant.WPF.Features.Phones;

public sealed class PhonesRepository : IPhonesRepository
{
    private readonly PhoneAssistantDbContext _dbContext;

    public PhonesRepository(PhoneAssistantDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<Phone>> AllAsync()
    {
        List<PhoneEntity> MobilePhones = await _dbContext.MobilePhones.ToListAsync();
        List<Phone> phones = new List<Phone>();
        foreach (PhoneEntity mobile in MobilePhones)
        {
            phones.Add(Phone.ToPhone(mobile));
        }
        return phones;
    }

    public async Task<IEnumerable<Phone>> SearchAsync(string search)
    {
        List<PhoneEntity> MobilePhones = await _dbContext.MobilePhones
            .Where(p => p.IMEI.Contains(search) || p.AssetTag.Contains(search))
            .ToListAsync();
        List<Phone> phones = new List<Phone>();
        foreach (PhoneEntity mobile in MobilePhones)
        {
            phones.Add(Phone.ToPhone(mobile));
        }
        return phones;
    }

    public async Task UpdateAsync(Phone phoneToUpdate)
    {
        PhoneEntity phone = _dbContext.MobilePhones.Where(mp => mp.Id == phoneToUpdate.Id).First();
        
        phone.IMEI = phoneToUpdate.IMEI;
        phone.FormerUser = phoneToUpdate.FormerUser;
        phone.Wiped = phoneToUpdate.Wiped;
        phone.Status = phoneToUpdate.Status;
        phone.OEM = phoneToUpdate.OEM;
        phone.AssetTag = phoneToUpdate.AssetTag;
        phone.Note = phoneToUpdate.Note;

        _dbContext.MobilePhones.Update(phone);
        await _dbContext.SaveChangesAsync();
        return;
    }
}