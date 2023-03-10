using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using PhoneAssistant.WPF.Features.Phones;
using PhoneAssistant.WPF.Features.ServiceRequests;
using PhoneAssistant.WPF.Features.Settings;
using PhoneAssistant.WPF.Features.Sims;
using PhoneAssistant.WPF.Shared;

namespace PhoneAssistant.WPF.Features.MainWindow;

public enum ViewModelType
{
    Dashboard,
    Phones,
    Sims,
    ServiceRequests,
    Settings
}

public sealed partial class MainWindowViewModel : ObservableObject, IViewModel
{
    private readonly PhonesMainViewModel _phonesMainViewModel;
    private readonly SimsMainViewModel _simCardMainViewModel;
    private readonly ServiceRequestsMainViewModel _serviceRequestMainViewModel;
    private readonly SettingsMainViewModel _settingsMainViewModel;

    [ObservableProperty]
    private IViewModel? _selectedViewModel;

    public MainWindowViewModel(PhonesMainViewModel phonesMainViewModel,
                               SimsMainViewModel simCardMainViewModel,
                               ServiceRequestsMainViewModel serviceRequestMainViewModel,
                               SettingsMainViewModel settingsMainViewModel)
    {
        _phonesMainViewModel = phonesMainViewModel;
        _simCardMainViewModel = simCardMainViewModel;
        _serviceRequestMainViewModel = serviceRequestMainViewModel;
        _settingsMainViewModel = settingsMainViewModel;
    }

    public string ViewPackIcon => throw new NotImplementedException();

    public string ViewName => throw new NotImplementedException();

    [RelayCommand]
    private async Task UpdateViewAsync(object selectedViewModelType)
    {
        if (selectedViewModelType is null) return;

        if (selectedViewModelType.GetType() != typeof(ViewModelType)) return;

        var viewType = (ViewModelType)selectedViewModelType;
        switch (viewType)
        {
            case ViewModelType.Dashboard:
                throw new NotImplementedException();
            case ViewModelType.Phones:
                SelectedViewModel = _phonesMainViewModel;
                break;
            case ViewModelType.Sims:
                SelectedViewModel = _simCardMainViewModel;
                break;
            case ViewModelType.ServiceRequests:
                SelectedViewModel = _serviceRequestMainViewModel;
                break;
            case ViewModelType.Settings:
                SelectedViewModel = _settingsMainViewModel;
                break;
            default: throw new NotImplementedException();
        }
        await LoadAsync();
    }

    public async Task LoadAsync()
    {
        await SelectedViewModel!.LoadAsync();
    }
}