using System.Windows;

using CommunityToolkit.Mvvm.Input;

namespace PhoneAssistant.WPF.Features.Users;
public sealed partial class UsersItemViewModel
{
    public string Name { get; }

    public string? Email { get; }

    public string LastLogonDate { get; set; }

    public string PasswordLastSet { get; set; }

    public string WhenCreated { get; set; }

    public bool Enabled { get; set; } = false;

    public UsersItemViewModel(User user)
    {
        Name = user.Name;
        Email = user.Email;
        LastLogonDate = user.Email;
        PasswordLastSet = user.PasswordLastSet;
        WhenCreated = user.WhenCreated;
        Enabled = user.Enabled;
    }

    [RelayCommand]
    public void CopyToClipbaord()
    {
        Clipboard.SetText(Email);
    }

}
