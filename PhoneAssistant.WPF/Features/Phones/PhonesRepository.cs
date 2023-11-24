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

    public async Task<string> UpdateAsync(v1Phone phone)
    {
        if (phone is null)
        {
            throw new ArgumentNullException(nameof(phone));
        }
        v1Phone? dbPhone = await _dbContext.Phones.FindAsync(phone.Imei);
        if (dbPhone is null)
        {
            throw new ArgumentException($"IMEI {phone.Imei} not found.");
        }
        dbPhone.AssetTag = phone.AssetTag;
        dbPhone.FormerUser = phone.FormerUser;
        dbPhone.Imei = phone.Imei;
        dbPhone.Model = phone.Model;
        dbPhone.NewUser = phone.NewUser;
        dbPhone.NorR = phone.NorR;
        dbPhone.Notes = phone.Notes;
        dbPhone.OEM = phone.OEM;
        dbPhone.PhoneNumber = phone.PhoneNumber;
        dbPhone.SimNumber = phone.SimNumber;
        dbPhone.SR = phone.SR;
        dbPhone.Status = phone.Status;

        _dbContext.Phones.Update(dbPhone);
        await _dbContext.SaveChangesAsync();

        v1Phone updatedPhone = await _dbContext.Phones.AsNoTracking().SingleAsync(x => x.Imei == phone.Imei);
        return updatedPhone.LastUpdate;
    }

    public async Task<string> UpdateKeyAsync(string oldImei, string newImei)
    {
        if (oldImei is null)
        {
            throw new ArgumentNullException(nameof(oldImei));
        }

        if (newImei is null)
        {
            throw new ArgumentNullException(nameof(newImei));
        }

        v1Phone? phone = await _dbContext.Phones.FindAsync(oldImei);
        if (phone is null)
        {
            throw new ArgumentException($"IMEI {oldImei} not found.");
        }

        _dbContext.Phones.Remove(phone);
        await _dbContext.SaveChangesAsync();

        phone.Imei = newImei;

        _dbContext.Phones.Add(phone);
        await _dbContext.SaveChangesAsync();

        v1Phone updatedPhone = await _dbContext.Phones.AsNoTracking().SingleAsync(x => x.Imei == phone.Imei);
        return updatedPhone.LastUpdate;        
    }

    public async Task<v1Phone> RemoveSimFromPhone(v1Phone phone)
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
            sim.SimNumber = phone.SimNumber;
            sim.Status = "In Stock";
            _dbContext.Sims.Update(sim);
        }
        else
        {        
            sim = new()
            {
                PhoneNumber = phone.PhoneNumber,
                SimNumber = phone.SimNumber,
                Status = "In Stock"        
            };
            _dbContext.Sims.Add(sim);
        }
        dbPhone.PhoneNumber = null;
        dbPhone.SimNumber = null;
        _dbContext.Phones.Update(dbPhone);
        await _dbContext.SaveChangesAsync();

        v1Phone updatedPhone = await _dbContext.Phones.AsNoTracking().SingleAsync(x => x.Imei == phone.Imei);
        return updatedPhone;        
    }
}