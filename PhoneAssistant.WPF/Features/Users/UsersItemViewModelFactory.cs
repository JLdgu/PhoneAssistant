namespace PhoneAssistant.WPF.Features.Users;

public interface IUsersItemViewModelFactory
{
    UsersItemViewModel Create(User user);
}

public sealed class UsersItemViewModelFactory : IUsersItemViewModelFactory
{
    public UsersItemViewModel Create(User user)
    { 
        return new UsersItemViewModel(user);
    }
}