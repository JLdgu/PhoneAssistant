using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using PhoneAssistant.WPF.Features.Dashboard;
using PhoneAssistant.WPF.Features.Phones;
using PhoneAssistant.WPF.Features.Settings;
using PhoneAssistant.WPF.Features.Users;
using PhoneAssistant.WPF.Shared;

namespace PhoneAssistant.WPF.Features.MainWindow;

public enum ViewModelType
{
    None,
    Dashboard,
    Phones,
    Sims,
    ServiceRequests,
    Settings,
    Users
}

public sealed partial class MainWindowViewModel : ObservableObject
{
    private readonly IDashboardMainViewModel _dashboardMainViewModel;
    private readonly IPhonesMainViewModel _phonesMainViewModel;
    private readonly ISettingsMainViewModel _settingsMainViewModel;
    private readonly IUsersMainViewModel _usersMainViewModel;
    [ObservableProperty]
#pragma warning disable IDE0044 // Add readonly modifier
    private IViewModel? _selectedViewModel;
#pragma warning restore IDE0044 // Add readonly modifier

    public MainWindowViewModel(IDashboardMainViewModel dashboardMainViewModel,
                               IPhonesMainViewModel phonesMainViewModel,
                               ISettingsMainViewModel settingsMainViewModel,
                               IUsersMainViewModel usersMainViewModel)
    {
        _dashboardMainViewModel = dashboardMainViewModel ?? throw new ArgumentNullException(nameof(dashboardMainViewModel));
        _phonesMainViewModel = phonesMainViewModel ?? throw new ArgumentNullException(nameof(phonesMainViewModel));
        _settingsMainViewModel = settingsMainViewModel ?? throw new ArgumentNullException(nameof(settingsMainViewModel));
        _usersMainViewModel = usersMainViewModel ?? throw new ArgumentNullException(nameof(usersMainViewModel));
    }

    [RelayCommand]
    private async Task UpdateViewAsync(object selectedViewModelType)
    {
        if (selectedViewModelType is null)
            throw new ArgumentNullException(nameof(selectedViewModelType));

        if (selectedViewModelType.GetType() != typeof(ViewModelType))
            throw new ArgumentException("Type " + selectedViewModelType.GetType() + " is not handled.");

        var viewType = (ViewModelType)selectedViewModelType;
        SelectedViewModel = viewType switch
        {
            ViewModelType.Dashboard => _dashboardMainViewModel,
            ViewModelType.Phones => _phonesMainViewModel,
            //ViewModelType.Sims => _simCardMainViewModel,
            //ViewModelType.ServiceRequests => _serviceRequestMainViewModel,
            ViewModelType.Settings => _settingsMainViewModel,
            ViewModelType.Users => _usersMainViewModel,
            _ => throw new NotImplementedException(),
        };
        await SelectedViewModel.LoadAsync();
    }

    public void WindowClosing()
    {
        if (SelectedViewModel is null) return;
        Task.Run(async () => await SelectedViewModel.WindowClosingAsync());
    }
}