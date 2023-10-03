namespace PhoneAssistant.WPF.Features.Users;

public interface IUsersItemViewModelFactory
{
    UsersItemViewModel Create(User user);
}