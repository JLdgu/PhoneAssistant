using PhoneAssistant.WPF.Shared;

namespace PhoneAssistant.WPF.Features.Users;
internal class UsersMainViewModel : IUsersMainViewModel
{
    public Task LoadAsync()
    {
        return Task.CompletedTask;
    }

    public Task WindowClosingAsync()
    {
        throw new NotImplementedException();
    }
}
