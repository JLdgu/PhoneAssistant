using System.Windows;

using CommunityToolkit.Mvvm.Input;

namespace PhoneAssistant.WPF.Features.Users;
public sealed partial class UsersItemViewModel
{
    public User User { get; }

    public UsersItemViewModel(User user)
    {
        if (user is null)
        {
            throw new ArgumentNullException(nameof(user));
        }
        User = user;
    }

    [RelayCommand]
    public void CopyNameToClipboard()
    {
        Clipboard.SetText(User.Name);
    }

    [RelayCommand(CanExecute =nameof(CanCopyEmailToClipbaord))]
    public void CopyEmailToClipboard()
    {
        Clipboard.SetText(User.Email);
    }

    private bool CanCopyEmailToClipbaord() => !string.IsNullOrEmpty(User.Email);
}
