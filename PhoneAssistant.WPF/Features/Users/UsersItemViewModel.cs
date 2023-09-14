using System.Windows;

using CommunityToolkit.Mvvm.Input;

namespace PhoneAssistant.WPF.Features.Users;
public sealed partial class UsersItemViewModel
{
    public string Name { get; }

    public string? Email { get; }

    public UsersItemViewModel(User user)
    {
        Name = user.Name;
        Email = user.Email;
    }

    [RelayCommand]
    public void CopyToClipbaord()
    {
        Clipboard.SetText(Email);
    }

}
