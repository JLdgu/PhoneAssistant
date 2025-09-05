using System.Collections.ObjectModel;
using System.DirectoryServices;
using System.Windows;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace PhoneAssistant.WPF.Features.Users;
public sealed partial class UsersMainViewModel : ObservableObject, IUsersMainViewModel
{
    private readonly IUsersItemViewModelFactory _usersItemViewModelFactory;
    public ObservableCollection<UsersItemViewModel> UserItems { get; } = new();

    public UsersMainViewModel(IUsersItemViewModelFactory usersItemViewModelFactory)
    {
        _usersItemViewModelFactory = usersItemViewModelFactory ?? throw new ArgumentNullException(nameof(usersItemViewModelFactory));
    }

    [ObservableProperty]
    private string? _searchUser;

    [RelayCommand]
    private async Task EnterKey()
    {
        if (string.IsNullOrEmpty(SearchUser)) return;

        UserItems.Clear();

        string person = SearchUser.Trim().Replace(" ", "*");
        
        ProgressVisibility = Visibility.Visible;

        await Task.Run(() =>
        {
            using SearchResultCollection results = PersonSearch(person);

            if (results.Count == 0)
            {
                NoResultsFound = true;
                return;
            }
            NoResultsFound = false;

            foreach (SearchResult sr in results)
            {
                var srp = sr.Properties["mail"];

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
    private SearchResultCollection PersonSearch(string name)
    {
        using DirectoryEntry entry = new DirectoryEntry("LDAP://DC=ds2,DC=devon,DC=gov,DC=uk");
        DirectorySearcher searcher = new DirectorySearcher(entry);
        searcher.Filter = $"(&(objectClass=user)(objectCategory=person)(CN=*{name}*))";
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
    private Visibility _progressVisibility = Visibility.Collapsed;

    [ObservableProperty]
    private bool _noResultsFound;

    private string ParsePropertyString(ResultPropertyValueCollection resultPropertyValueCollection)
    {
        if (resultPropertyValueCollection is null) return string.Empty;
        if (resultPropertyValueCollection.Count == 0) return string.Empty;
        return resultPropertyValueCollection[0].ToString() ?? string.Empty;
    }

    private string ParsePropertyDateTime(ResultPropertyValueCollection resultPropertyValueCollection)
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
