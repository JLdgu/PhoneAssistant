using System.Collections.ObjectModel;
using System.DirectoryServices;

using CommunityToolkit.Mvvm.ComponentModel;

namespace PhoneAssistant.WPF.Features.Users;
public sealed partial class UsersMainViewModel : ObservableObject, IUsersMainViewModel
{
    private IUsersItemViewModelFactory _usersItemViewModelFactory { get; }
    public ObservableCollection<UsersItemViewModel> UserItems { get; } = new();

    public UsersMainViewModel(IUsersItemViewModelFactory usersItemViewModelFactory)
    {
        _usersItemViewModelFactory = usersItemViewModelFactory;
    }

    [ObservableProperty]
    private string? _searchUser;

    partial void OnSearchUserChanged(string? value)
    {
        if (value == null) return;
        if (value.Length < 3) return;

        UserItems.Clear();

        using SearchResultCollection results = PersonSearch(value);

        if (results.Count == 0) 
        {
            NoResultsFound = true;
            return;
        }
        NoResultsFound = false;

        foreach (SearchResult sr in results)
        {
            var srp = sr.Properties["mail"];

            User user = new User()
            {
                Name = ParsePropertyString(sr.Properties["displayName"]),
                Description = ParsePropertyString(sr.Properties["description"])
            };
            user.Email = ParsePropertyString(sr.Properties["mail"]);
            user.LastLogonDate = ParsePropertyDateTime(sr.Properties["lastLogon"]);
            user.WhenCreated = ParsePropertyString(sr.Properties["whenCreated"]);
            user.PasswordLastSet = ParsePropertyDateTime(sr.Properties["pwdLastSet"]);
            int flags = (int)sr.Properties["userAccountControl"][0];
            UserAccountControl userAccountControl = (UserAccountControl)flags;
            user.Enabled = (userAccountControl & UserAccountControl.ACCOUNTDISABLE) != UserAccountControl.ACCOUNTDISABLE;            

            UserItems.Add(_usersItemViewModelFactory.Create(user));
        }
    }
    private SearchResultCollection PersonSearch(string name)
    {
        using DirectoryEntry entry = new DirectoryEntry("LDAP://DC=ds2,DC=devon,DC=gov,DC=uk");
        DirectorySearcher searcher = new DirectorySearcher(entry);
        searcher.Filter = $"(&(objectClass=user)(objectCategory=person)(CN=*{name}*))";
        searcher.PropertiesToLoad.Add("displayName");
        searcher.PropertiesToLoad.Add("description");
        searcher.PropertiesToLoad.Add("lastLogon");
        searcher.PropertiesToLoad.Add("mail");
        searcher.PropertiesToLoad.Add("whenCreated");
        searcher.PropertiesToLoad.Add("pwdLastSet");
        searcher.PropertiesToLoad.Add("userAccountControl");

        searcher.Sort.PropertyName = "displayName";
        return searcher.FindAll();
    }


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

    public Task WindowClosingAsync()
    {
        throw new NotImplementedException();
    }
}
