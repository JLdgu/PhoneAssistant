namespace PhoneAssistant.Model;
public interface IBaseReportRepository
{
    Task CreateAsync(BaseReport report);
    Task<IEnumerable<BaseReport>> GetBaseReportAsync();
    Task<string?> GetSimNumberAsync(string phoneNumber);
    Task TruncateAsync();
}