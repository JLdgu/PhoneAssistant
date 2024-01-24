using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using PhoneAssistant.WPF.Features.BaseReport;
using PhoneAssistant.WPF.Features.Dashboard;
using PhoneAssistant.WPF.Features.Disposals;
using PhoneAssistant.WPF.Features.Phones;
using PhoneAssistant.WPF.Features.Settings;
using PhoneAssistant.WPF.Features.Sims;
using PhoneAssistant.WPF.Features.Users;
using PhoneAssistant.WPF.Shared;

namespace PhoneAssistant.WPF.Features.MainWindow;

public enum ViewModelType
{
    None,
    BaseReport,
    Dashboard,
    Disposals,
    Phones,
    Sims,
    ServiceRequests,
    Settings,
    Users
}

public sealed partial class MainWindowViewModel : ObservableObject
{
    private readonly IBaseReportMainViewModel _baseReportMainViewModel;
    private readonly IDashboardMainViewModel _dashboardMainViewModel;
    private readonly IDisposalsMainViewModel _disposalsMainViewModel;
    private readonly IPhonesMainViewModel _phonesMainViewModel;
    private readonly ISimsMainViewModel _simsMainViewModel;
    private readonly ISettingsMainViewModel _settingsMainViewModel;
    private readonly IUsersMainViewModel _usersMainViewModel;

    [ObservableProperty]
    private IViewModel? _selectedViewModel;

    public MainWindowViewModel(IBaseReportMainViewModel baseReportMainViewModel,
                               IDashboardMainViewModel dashboardMainViewModel,
                               IDisposalsMainViewModel disposalsMainViewModel,
                               IPhonesMainViewModel phonesMainViewModel,
                               ISimsMainViewModel simsMainViewModel,
                               ISettingsMainViewModel settingsMainViewModel,
                               IUsersMainViewModel usersMainViewModel)
    {
        _baseReportMainViewModel = baseReportMainViewModel ?? throw new ArgumentNullException(nameof(baseReportMainViewModel));
        _dashboardMainViewModel = dashboardMainViewModel ?? throw new ArgumentNullException(nameof(dashboardMainViewModel));
        _disposalsMainViewModel = disposalsMainViewModel ?? throw new ArgumentNullException(nameof(disposalsMainViewModel));
        _phonesMainViewModel = phonesMainViewModel ?? throw new ArgumentNullException(nameof(phonesMainViewModel));
        _simsMainViewModel = simsMainViewModel ?? throw new ArgumentNullException(nameof(simsMainViewModel));
        _settingsMainViewModel = settingsMainViewModel ?? throw new ArgumentNullException(nameof(settingsMainViewModel));
        _usersMainViewModel = usersMainViewModel ?? throw new ArgumentNullException(nameof(usersMainViewModel));

        SelectedViewModel = _dashboardMainViewModel;
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
            ViewModelType.BaseReport => _baseReportMainViewModel,
            ViewModelType.Dashboard => _dashboardMainViewModel,
            ViewModelType.Disposals => _disposalsMainViewModel,
            ViewModelType.Phones => _phonesMainViewModel,
            ViewModelType.Sims => _simsMainViewModel,
            //ViewModelType.ServiceRequests => _serviceRequestMainViewModel,
            ViewModelType.Settings => _settingsMainViewModel,
            ViewModelType.Users => _usersMainViewModel,
            _ => throw new NotImplementedException(),
        };
        await SelectedViewModel.LoadAsync();
    }
}