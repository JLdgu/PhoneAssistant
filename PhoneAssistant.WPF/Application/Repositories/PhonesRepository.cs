using Microsoft.EntityFrameworkCore;

using PhoneAssistant.WPF.Application;
using PhoneAssistant.WPF.Application.Entities;

namespace PhoneAssistant.WPF.Application.Repositories;

public sealed class PhonesRepository : IPhonesRepository
{
    private readonly PhoneAssistantDbContext _dbContext;

    public PhonesRepository(PhoneAssistantDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> AssetTagUniqueAsync(string? assetTag)
    {
        Phone? found = await _dbContext.Phones.Where(p => p.AssetTag == assetTag).FirstOrDefaultAsync();
        return found == null;
    }

    public async Task CreateAsync(Phone phone)
    {
        _dbContext.Phones.Add(phone);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(string imei)
    {
        Phone? found = await _dbContext.Phones.FindAsync(imei);
        return found != null;
    }

    public async Task<IEnumerable<Phone>> GetActivePhonesAsync()
    {
        IEnumerable<Phone> phones = await _dbContext.Phones
            .Where(p => p.Status != "Disposed" &&  p.Status != "Decommissioned")                               
            .AsNoTracking()
            .OrderByDescending(p => p.LastUpdate)
            .ToListAsync();
        return phones;
    }
    
    public async Task<IEnumerable<Phone>> GetAllPhonesAsync()
    {
        IEnumerable<Phone> phones = await _dbContext.Phones
            .ToListAsync();
        return phones;
    }
    public async Task RemoveSimFromPhoneAsync(Phone phone)
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

        await UpdateHistoryAsync(phone, UpdateTypes.UPDATE);        

        Phone dbPhone = await _dbContext.Phones.SingleAsync(x => x.Imei == phone.Imei);
        Sim? sim = await _dbContext.Sims.FindAsync(phone.PhoneNumber);
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
        phone.PhoneNumber = null;
        dbPhone.SimNumber = null;
        phone.SimNumber = null;
        dbPhone.SetLastUpdate();
        phone.SetLastUpdate();
        _dbContext.Phones.Update(dbPhone);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(Phone phone)
    {
        if (phone is null)
        {
            throw new ArgumentNullException(nameof(phone));
        }
        Phone? dbPhone = await _dbContext.Phones.FindAsync(phone.Imei);
        if (dbPhone is null)
        {
            throw new ArgumentException($"IMEI {phone.Imei} not found.");
        }

        await UpdateHistoryAsync(phone, UpdateTypes.UPDATE);        

        dbPhone.AssetTag = phone.AssetTag;
        dbPhone.DespatchDetails = phone.DespatchDetails;
        dbPhone.FormerUser = phone.FormerUser;
        dbPhone.Imei = phone.Imei;
        dbPhone.Model = phone.Model;
        dbPhone.NewUser = phone.NewUser;
        dbPhone.Condition = phone.Condition;
        dbPhone.Notes = phone.Notes;
        dbPhone.OEM = phone.OEM;
        dbPhone.PhoneNumber = phone.PhoneNumber;
        dbPhone.SimNumber = phone.SimNumber;
        dbPhone.SR = phone.SR;
        dbPhone.Status = phone.Status;
        dbPhone.SetLastUpdate();
        phone.SetLastUpdate();
        _dbContext.Phones.Update(dbPhone);

        await _dbContext.SaveChangesAsync();
    }

    private async Task UpdateHistoryAsync(Phone phone, UpdateTypes updateType)
    {
        UpdateHistoryPhone? history = await _dbContext.UpdateHistoryPhones
            .OrderByDescending(h =>h.Id)
            .FirstOrDefaultAsync(h => h.Imei == phone.Imei);

        if (history is not null) 
        {
            if (phone.AssetTag == history.AssetTag &&
                phone.Condition == history.Condition &&
                phone.DespatchDetails == history.DespatchDetails &&
                phone.FormerUser == history.FormerUser &&
                phone.Model == history.Model &&
                phone.NewUser == history.NewUser &&
                phone.Notes == history.Notes &&
                phone.OEM == history.OEM &&
                phone.PhoneNumber == history.PhoneNumber &&
                phone.SimNumber == history.SimNumber &&
                phone.SR == history.SR &&   
                phone.Status == history.Status)
                return;
        }

        history = new(phone, updateType);
        _dbContext.UpdateHistoryPhones.Add(history);
    }
    //public async Task<string> UpdateKeyAsync(string oldImei, string newImei)
    //{
    //    if (oldImei is null)
    //    {
    //        throw new ArgumentNullException(nameof(oldImei));
    //    }

    //    if (newImei is null)
    //    {
    //        throw new ArgumentNullException(nameof(newImei));
    //    }

    //    Phone? phone = await _dbContext.Phones.FindAsync(oldImei);
    //    if (phone is null)
    //    {
    //        throw new ArgumentException($"IMEI {oldImei} not found.");
    //    }

    //    _dbContext.Phones.Remove(phone);
    //    await _dbContext.SaveChangesAsync();

    //    phone.Imei = newImei;

    //    _dbContext.Phones.Add(phone);
    //    await _dbContext.SaveChangesAsync();

    //    Phone updatedPhone = await _dbContext.Phones.AsNoTracking().SingleAsync(x => x.Imei == phone.Imei);
    //    return updatedPhone.LastUpdate;
    //}
}