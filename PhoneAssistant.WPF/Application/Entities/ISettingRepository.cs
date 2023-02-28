using System.Threading.Tasks;

namespace PhoneAssistant.WPF.Application.Entities;
public interface ISettingRepository
{
    Task<string> GetAsync();
}