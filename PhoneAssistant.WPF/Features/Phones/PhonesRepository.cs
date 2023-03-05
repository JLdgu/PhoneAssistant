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

    public void SaveChanges(Phone changedPhone)
    {
        PhoneEntity mobile = changedPhone;
        //_dbContext.MobilePhones.Update(mobile);
        //_dbContext.SaveChanges();
    }

    public async Task<IEnumerable<Phone>> SearchAsync(string search)
    {
        List<PhoneEntity> MobilePhones = await _dbContext.MobilePhones
            .Where(p => p.IMEI.Contains(search) || p.AssetTag.Contains(search) )
            .ToListAsync();
        List<Phone> phones = new List<Phone>();
        foreach (PhoneEntity mobile in MobilePhones)
        {
            phones.Add(Phone.ToPhone(mobile));
        }
        return phones;
    }
}