using System.Windows;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using PhoneAssistant.Model;
using PhoneAssistant.WPF.Features.AddItem;
using PhoneAssistant.WPF.Features.BaseReport;
using PhoneAssistant.WPF.Features.Dashboard;
using PhoneAssistant.WPF.Features.Disposals;
using PhoneAssistant.WPF.Features.Dymo;
using PhoneAssistant.WPF.Features.Phones;
using PhoneAssistant.WPF.Features.Settings;
using PhoneAssistant.WPF.Features.Sims;
using PhoneAssistant.WPF.Features.Users;
using PhoneAssistant.WPF.Shared;

namespace PhoneAssistant.WPF.Features.MainWindow;

public enum ViewModelType
{
    None = 0,
    AddItem,
    BaseReport,
    Dashboard,
    Disposals,
    Dymo,
    Phones,
    Sims,
    Settings,
    Users
}

public sealed partial class MainWindowViewModel : ObservableObject
{
    private readonly IAddItemViewModel _addItemViewModel;
    private readonly IApplicationSettingsRepository _appSettings;
    private readonly IBaseReportMainViewModel _baseReportMainViewModel;
    private readonly IDashboardMainViewModel _dashboardMainViewModel;
    private readonly IDisposalsMainViewModel _disposalsMainViewModel;
    private readonly DymoViewModel _dymoViewModel;
    private readonly IPhonesMainViewModel _phonesMainViewModel;
    private readonly ISimsMainViewModel _simsMainViewModel;
    private readonly ISettingsMainViewModel _settingsMainViewModel;
    private readonly IUsersMainViewModel _usersMainViewModel;

    public MainWindowViewModel(IAddItemViewModel addItemViewModel,
                               IApplicationSettingsRepository appSettings,
                               IBaseReportMainViewModel baseReportMainViewModel,
                               IDashboardMainViewModel dashboardMainViewModel,
                               IDisposalsMainViewModel disposalsMainViewModel,
                               DymoViewModel dymoViewModel,
                               IPhonesMainViewModel phonesMainViewModel,
                               ISimsMainViewModel simsMainViewModel,
                               ISettingsMainViewModel settingsMainViewModel,
                               IUsersMainViewModel usersMainViewModel)
    {
        _addItemViewModel = addItemViewModel ?? throw new ArgumentNullException(nameof(addItemViewModel));
        _appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));
        _baseReportMainViewModel = baseReportMainViewModel ?? throw new ArgumentNullException(nameof(baseReportMainViewModel));
        _dashboardMainViewModel = dashboardMainViewModel ?? throw new ArgumentNullException(nameof(dashboardMainViewModel));
        _disposalsMainViewModel = disposalsMainViewModel ?? throw new ArgumentNullException(nameof(disposalsMainViewModel));
        _dymoViewModel = dymoViewModel ?? throw new ArgumentNullException(nameof(dymoViewModel));
        _phonesMainViewModel = phonesMainViewModel ?? throw new ArgumentNullException(nameof(phonesMainViewModel));
        _simsMainViewModel = simsMainViewModel ?? throw new ArgumentNullException(nameof(simsMainViewModel));
        _settingsMainViewModel = settingsMainViewModel ?? throw new ArgumentNullException(nameof(settingsMainViewModel));
        _usersMainViewModel = usersMainViewModel ?? throw new ArgumentNullException(nameof(usersMainViewModel));

#if DEBUG
        Development = true;
#endif
        if (_settingsMainViewModel.UpdateState == ApplicationUpdateState.UpdateAvailable)
            ShowUpdateBadge = Visibility.Visible;

        SelectedView = (ViewModelType)appSettings.ApplicationSettings.CurrentView;
        _ = UpdateViewAsync(SelectedView);
    }

    [ObservableProperty]
    private bool _development = false;

    [ObservableProperty]
    private ViewModelType _selectedView;

    [ObservableProperty]
    private IViewModel? _selectedViewModel;

    [RelayCommand]
    private async Task UpdateViewAsync(ViewModelType selectedViewModelType)
    {
        ArgumentNullException.ThrowIfNull(selectedViewModelType);

        SelectedViewModel = selectedViewModelType switch
        {
            ViewModelType.None => _dashboardMainViewModel,
            ViewModelType.AddItem => _addItemViewModel,
            ViewModelType.BaseReport => _baseReportMainViewModel,
            ViewModelType.Dashboard => _dashboardMainViewModel,
            ViewModelType.Disposals => _disposalsMainViewModel,
            ViewModelType.Dymo => _dymoViewModel,
            ViewModelType.Phones => _phonesMainViewModel,
            ViewModelType.Sims => _simsMainViewModel,
            ViewModelType.Settings => _settingsMainViewModel,
            ViewModelType.Users => _usersMainViewModel,
            _ => throw new NotImplementedException()
        };
        _appSettings.ApplicationSettings.CurrentView = (int)selectedViewModelType;
        _appSettings.Save();

        await SelectedViewModel.LoadAsync();
    }

    [ObservableProperty]
    private Visibility _showUpdateBadge = Visibility.Collapsed;
}