using System.Windows;

using CommunityToolkit.Mvvm.Input;

namespace PhoneAssistant.WPF.Features.Users;
public sealed partial class UsersItemViewModel
{
    public string Name { get; }

    public string? Description { get; }

    public string? Email { get; }

    public string LastLogonDate { get; set; }

    public string PasswordLastSet { get; set; }

    public string WhenCreated { get; set; }

    public bool Enabled { get; set; } = false;

    public UsersItemViewModel(User user)
    {
        Name = user.Name;
        Description = user.Description;
        Email = user.Email;
        LastLogonDate = user.LastLogonDate;
        PasswordLastSet = user.PasswordLastSet;
        WhenCreated = user.WhenCreated;
        Enabled = user.Enabled;
    }

    [RelayCommand]
    public void CopyNameToClipboard()
    {
        Clipboard.SetText(Name);
    }

    [RelayCommand(CanExecute =nameof(CanCopyEmailToClipbaord))]
    public void CopyEmailToClipboard()
    {
        Clipboard.SetText(Email);
    }

    private bool CanCopyEmailToClipbaord() => !string.IsNullOrEmpty(Email);
}
