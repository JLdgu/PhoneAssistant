using System.Collections.ObjectModel;
using System.DirectoryServices;

using CommunityToolkit.Mvvm.ComponentModel;

namespace PhoneAssistant.WPF.Features.Users;
public  sealed partial class UsersMainViewModel : ObservableObject, IUsersMainViewModel
{
    public ObservableCollection<User> Users { get; } = new();

    [ObservableProperty]
    private string? _searchUser;

    partial void OnSearchUserChanged(string? value)
    {
        if (value == null) return;
        if (value.Length < 3) return;

        Users.Clear();

        using DirectoryEntry entry = new DirectoryEntry("LDAP://DC=ds2,DC=devon,DC=gov,DC=uk");
        DirectorySearcher searcher = new DirectorySearcher(entry);
        searcher.Filter = $"(&(objectClass=user)(objectCategory=person)(CN=*{value}*))";
        searcher.PropertiesToLoad.Add("displayName");
        searcher.PropertiesToLoad.Add("lastLogon");
        searcher.PropertiesToLoad.Add("mail");
        searcher.PropertiesToLoad.Add("whenCreated");
        searcher.PropertiesToLoad.Add("pwdLastSet");
        searcher.PropertiesToLoad.Add("userAccountControl");

        SearchResultCollection results = searcher.FindAll();

        foreach (SearchResult sr in results)
        {
            var srp = sr.Properties["mail"];

            User user = new User()
            {
                Name = ParsePropertyString(sr.Properties["displayName"])
            };
            user.Email = ParsePropertyString(sr.Properties["mail"]);
            user.LastLogonDate = ParsePropertyDateTime(sr.Properties["lastLogon"]);
            user.WhenCreated = ParsePropertyString(sr.Properties["whenCreated"]);
            user.PasswordLastSet = ParsePropertyDateTime(sr.Properties["pwdLastSet"]);
            Users.Add(user);
        }

    }

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
