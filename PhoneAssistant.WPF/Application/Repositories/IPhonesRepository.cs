namespace PhoneAssistant.Model;

public interface IPhonesRepository
{
    Task<bool> AssetTagUniqueAsync(string? assetTag);
    Task CreateAsync(Phone phone);
    Task<bool> ExistsAsync(string imei);
    Task<IEnumerable<Phone>> GetActivePhonesAsync();
    Task<IEnumerable<Phone>> GetAllPhonesAsync();
    Task<Phone?> GetPhoneAsync(string imei);
    Task<bool> PhoneNumberExistsAsync(string phoneNumber);
    Task UpdateAsync(Phone phone);
    Task UpdateStatusAsync(string imei, string status);
}