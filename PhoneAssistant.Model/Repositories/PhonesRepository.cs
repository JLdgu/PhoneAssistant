using Microsoft.EntityFrameworkCore;

namespace PhoneAssistant.Model;

public sealed class PhonesRepository(PhoneAssistantDbContext dbContext) : IPhonesRepository
{
    public async Task<bool> AssetTagUniqueAsync(string? assetTag)
    {
        Phone? found = await dbContext.Phones.Where(p => p.AssetTag == assetTag).FirstOrDefaultAsync();
        return found == null;
    }

    public async Task CreateAsync(Phone phone)
    {
        dbContext.Phones.Add(phone);
        await dbContext.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(string imei)
    {
        Phone? found = await dbContext.Phones.FindAsync(imei);
        return found != null;
    }

    public async Task<IEnumerable<Phone>> GetActivePhonesAsync()
    {
        IEnumerable<Phone> phones = await dbContext.Phones
            .Where(p => p.Status != "Disposed" && p.Status != "Decommissioned")
            .OrderByDescending(p => p.LastUpdate)
            .AsNoTracking()
            .ToListAsync();
        return phones;
    }

    public async Task<IEnumerable<Phone>> GetAllPhonesAsync()
    {
        IEnumerable<Phone> phones = await dbContext.Phones
            .OrderByDescending(p => p.LastUpdate)
            .AsNoTracking()
            .ToListAsync();
        return phones;
    }

    public async Task<Phone?> GetPhoneAsync(string imei)
    {
        return await dbContext.Phones.FindAsync(imei);
    }

    public async Task<bool> PhoneNumberExistsAsync(string phoneNumber)
    {
        Phone? phone = await dbContext.Phones.FirstOrDefaultAsync(p => p.PhoneNumber == phoneNumber);

        return phone is not null;
    }

    public async Task<Result> UpdateAsync(Phone phone)
    {
        Phone? dbPhone = await dbContext.Phones.FindAsync(phone.Imei) ?? throw new ArgumentException($"IMEI {phone.Imei} not found.");
        Result result = Result.Unchanged;

        if (dbPhone.AssetTag != phone.AssetTag)
        {
            dbPhone.AssetTag = phone.AssetTag;
            result = Result.Updated;
        }
        if (dbPhone.Condition != phone.Condition)
        {
            dbPhone.Condition = phone.Condition;
            result = Result.Updated;
        }
        if (dbPhone.DespatchDetails != phone.DespatchDetails)
        {
            dbPhone.DespatchDetails = phone.DespatchDetails;
            result = Result.Updated;
        }
        if (dbPhone.Esim != phone.Esim)
        {
            dbPhone.Esim = phone.Esim;
            result = Result.Updated;
        }
        if (dbPhone.FormerUser != phone.FormerUser)
        {
            dbPhone.FormerUser = phone.FormerUser;
            result = Result.Updated;
        }
        if (dbPhone.IncludeOnTrackingSheet != phone.IncludeOnTrackingSheet)
        {
            dbPhone.IncludeOnTrackingSheet = phone.IncludeOnTrackingSheet;
            result = Result.Updated;
        }
        if (dbPhone.Model != phone.Model)
        {
            dbPhone.Model = phone.Model;
            result = Result.Updated;
        }
        if (dbPhone.NewUser != phone.NewUser)
        {
            dbPhone.NewUser = phone.NewUser;
            result = Result.Updated;
        }
        if (dbPhone.Notes != phone.Notes)
        {
            dbPhone.Notes = phone.Notes;
            result = Result.Updated;
        }
        if (dbPhone.OEM != phone.OEM)
        {
            dbPhone.OEM = phone.OEM;
            result = Result.Updated;
        }
        if (dbPhone.PhoneNumber != phone.PhoneNumber)
        {
            dbPhone.PhoneNumber = phone.PhoneNumber;
            result = Result.Updated;
        }
        if (dbPhone.SerialNumber != phone.SerialNumber)
        {
            dbPhone.SerialNumber = phone.SerialNumber;
            result = Result.Updated;
        }
        if (dbPhone.SimNumber != phone.SimNumber)
        {
            dbPhone.SimNumber = phone.SimNumber;
            result = Result.Updated;
        }
        if (dbPhone.Status != phone.Status)
        {
            dbPhone.Status = phone.Status;
            result = Result.Updated;
        }
        if (dbPhone.Ticket != phone.Ticket)
        {
            dbPhone.Ticket = phone.Ticket;
            result = Result.Updated;
        }

        dbContext.Phones.Update(dbPhone);
        await dbContext.SaveChangesAsync();
        phone.LastUpdate = dbPhone.LastUpdate;

        return result;
    }

    public async Task UpdateStatusAsync(string imei, string status)
    {
        ArgumentNullException.ThrowIfNull(imei);
        ArgumentNullException.ThrowIfNull(status);
        Phone? dbPhone = await dbContext.Phones.FindAsync(imei) ?? throw new ArgumentException($"IMEI {imei} not found.");

        dbPhone.Status = status;
        await UpdateAsync(dbPhone);
    }
}
