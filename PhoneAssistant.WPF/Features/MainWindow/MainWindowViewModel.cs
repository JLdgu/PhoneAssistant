using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Microsoft.EntityFrameworkCore.Diagnostics;

using PhoneAssistant.WPF.Features.Dashboard;
using PhoneAssistant.WPF.Features.Phones;
using PhoneAssistant.WPF.Features.ServiceRequests;
using PhoneAssistant.WPF.Features.Settings;
using PhoneAssistant.WPF.Features.Sims;
using PhoneAssistant.WPF.Shared;

namespace PhoneAssistant.WPF.Features.MainWindow;

public enum ViewModelType
{
    None,
    Dashboard,
    Phones,
    Print,
    Sims,
    ServiceRequests,
    Settings
}

public sealed partial class MainWindowViewModel : ObservableObject
{
    private readonly IDashboardMainViewModel _dashboardMainViewModel;
    private readonly IPhonesMainViewModel _phonesMainViewModel;
    //private readonly ISimsMainViewModel _simCardMainViewModel;
    //private readonly IServiceRequestsMainViewModel _serviceRequestMainViewModel;
    private readonly ISettingsMainViewModel _settingsMainViewModel;

    [ObservableProperty]
#pragma warning disable IDE0044 // Add readonly modifier
    private IViewModel? _selectedViewModel;
#pragma warning restore IDE0044 // Add readonly modifier

    public MainWindowViewModel(IDashboardMainViewModel dashboardMainViewModel,
                               IPhonesMainViewModel phonesMainViewModel,
                               //ISimsMainViewModel simCardMainViewModel,
                               //IServiceRequestsMainViewModel serviceRequestMainViewModel,
                               ISettingsMainViewModel settingsMainViewModel)
    {
        _dashboardMainViewModel = dashboardMainViewModel ?? throw new ArgumentNullException(nameof(dashboardMainViewModel));
        _phonesMainViewModel = phonesMainViewModel ?? throw new ArgumentNullException(nameof(phonesMainViewModel));
        //_simCardMainViewModel = simCardMainViewModel ?? throw new ArgumentNullException(nameof(simCardMainViewModel));
        //_serviceRequestMainViewModel = serviceRequestMainViewModel;
        _settingsMainViewModel = settingsMainViewModel ?? throw new ArgumentNullException(nameof(settingsMainViewModel));
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