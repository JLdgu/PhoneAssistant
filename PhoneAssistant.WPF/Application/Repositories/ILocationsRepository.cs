namespace PhoneAssistant.Model;

public interface ILocationsRepository
{
    Task<IEnumerable<Location>> GetAllLocationsAsync();
}