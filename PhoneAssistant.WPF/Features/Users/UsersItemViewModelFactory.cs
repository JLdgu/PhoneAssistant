namespace PhoneAssistant.WPF.Features.Users;
public sealed class UsersItemViewModelFactory : IUsersItemViewModelFactory
{
    public UsersItemViewModel Create(User user)
    { 
        return new UsersItemViewModel(user); ; 
    }
}