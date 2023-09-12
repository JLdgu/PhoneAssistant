using PhoneAssistant.WPF.Shared;

namespace PhoneAssistant.WPF.Features.Users;

public interface IUsersMainViewModel : IViewModel
{
    public string SearchUser { get; set; }
}