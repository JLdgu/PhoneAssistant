using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.DirectoryServices;
using System.Windows;

using PhoneAssistant.WPF.Shared;

namespace PhoneAssistant.WPF.Features.Users;

public interface IUsersMainViewModel : IViewModel
{
    public string? SearchUserName { get; set; }
}

public sealed partial class UsersMainViewModel : ObservableObject, IUsersMainViewModel
{
    private readonly IUsersItemViewModelFactory _usersItemViewModelFactory;
    public ObservableCollection<UsersItemViewModel> UserItems { get; } = [];

    public UsersMainViewModel(IUsersItemViewModelFactory usersItemViewModelFactory) => _usersItemViewModelFactory = usersItemViewModelFactory ?? throw new ArgumentNullException(nameof(usersItemViewModelFactory));

    [ObservableProperty]
    public partial string? SearchUserEmail { get; set; }

    [ObservableProperty]
    public partial string? SearchUserName { get; set; }

    private async Task Search(string ldapFilter)
    {
        UserItems.Clear();
        ProgressVisibility = Visibility.Visible;
        NoResultsFound = false;

        await Task.Run(() =>
        {
            using SearchResultCollection results = PersonSearch(ldapFilter);

            if (results.Count == 0)
            {
                NoResultsFound = true;
                return;
            }

            foreach (SearchResult sr in results)
            {
                User user = new()
                {
                    Name = ParsePropertyString(sr.Properties["displayName"]),
                    Description = ParsePropertyString(sr.Properties["description"]),
                    Email = ParsePropertyString(sr.Properties["mail"]),
                    LastLogonDate = ParsePropertyDateTime(sr.Properties["lastLogon"]),
                    WhenCreated = ParsePropertyString(sr.Properties["whenCreated"]),
                    PasswordLastSet = ParsePropertyDateTime(sr.Properties["pwdLastSet"])
                };
                if (string.IsNullOrEmpty(user.LastLogonDate))
                {
                    user.LastLogonDate = ParsePropertyDateTime(sr.Properties["lastLogonTimestamp"]);
                }
                int flags = (int)sr.Properties["userAccountControl"][0];
                UserAccountControl userAccountControl = (UserAccountControl)flags;
                user.Enabled = (userAccountControl & UserAccountControl.ACCOUNTDISABLE) != UserAccountControl.ACCOUNTDISABLE;

                App.Current.Dispatcher.Invoke((Action)delegate
                {
                    UserItems.Add(_usersItemViewModelFactory.Create(user));
                });
            }
        });

        ProgressVisibility = Visibility.Collapsed;
    }

    [RelayCommand]
    private async Task SearchEmail()
    {
        if (string.IsNullOrEmpty(SearchUserEmail)) return;

        string filter = SearchUserEmail.Trim();

        await Search($"mail=*{filter}*");

    }

    [RelayCommand]
    private async Task SearchName()
    {
        if (string.IsNullOrEmpty(SearchUserName)) return;

        string filter = SearchUserName.Trim().Replace(" ", "*");

        await Search($"displayName=*{filter}*");
    }

    private static SearchResultCollection PersonSearch(string filter)
    {
        using DirectoryEntry entry = new("LDAP://ds2.devon.gov.uk");
        entry.AuthenticationType = AuthenticationTypes.Secure;
        DirectorySearcher searcher = new(entry)
        {
            Filter = $"(&(objectClass=user)(objectCategory=person)({filter}))"
        };
        searcher.PropertiesToLoad.Add("displayName");
        searcher.PropertiesToLoad.Add("description");
        searcher.PropertiesToLoad.Add("lastLogon");
        searcher.PropertiesToLoad.Add("lastLogonTimestamp");
        searcher.PropertiesToLoad.Add("mail");
        searcher.PropertiesToLoad.Add("whenCreated");
        searcher.PropertiesToLoad.Add("pwdLastSet");
        searcher.PropertiesToLoad.Add("userAccountControl");

        searcher.Sort.PropertyName = "displayName";
        return searcher.FindAll();
    }

    [ObservableProperty]
    public partial Visibility ProgressVisibility { get; set; } = Visibility.Collapsed;

    [ObservableProperty]
    public partial bool NoResultsFound { get; set; }

    private static string ParsePropertyString(ResultPropertyValueCollection resultPropertyValueCollection)
    {
        if (resultPropertyValueCollection is null) return string.Empty;
        if (resultPropertyValueCollection.Count == 0) return string.Empty;
        return resultPropertyValueCollection[0].ToString() ?? string.Empty;
    }

    private static string ParsePropertyDateTime(ResultPropertyValueCollection resultPropertyValueCollection)
    {
        if (resultPropertyValueCollection is null) return string.Empty;
        if (resultPropertyValueCollection.Count == 0) return string.Empty;
        long l = (long)resultPropertyValueCollection[0];
        //if (l ==0) return string.Empty;
        DateTime dt = DateTime.FromFileTime(l);
        if (dt.Date.Equals(MinFileTime)) return string.Empty;
        return dt.ToString();
    }

    private static readonly DateTime MinFileTime = DateTime.FromFileTime(0);

    public Task LoadAsync()
    {
        return Task.CompletedTask;
    }
}
