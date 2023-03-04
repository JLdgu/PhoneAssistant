using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PhoneAssistant.WPF.Application;
using PhoneAssistant.WPF.Application.Entities;
using PhoneAssistant.WPF.Features.ServiceRequest;
using PhoneAssistant.WPF.Features.Settings;
using PhoneAssistant.WPF.Features.SimCard;
using PhoneAssistant.WPF.Features.SmartPhone;
using PhoneAssistant.WPF.Models;
using PhoneAssistant.WPF.Shared;

namespace PhoneAssistant.WPF.Features.MainWindow;

public enum ViewType
{
    Dashboard,
    Phone,
    SimCard,
    ServiceRequest,
    Settings
}

public sealed partial class MainWindowViewModel : ObservableObject, IViewModel
{
    private readonly SmartPhoneMainViewModel _smartPhoneMainViewModel;
    private readonly SimCardMainViewModel _simCardMainViewModel;
    private readonly ServiceRequestMainViewModel _serviceRequestMainViewModel;
    private readonly SettingsMainViewModel _settingsMainViewModel;

    [ObservableProperty]
    private IViewModel? _selectedViewModel;

    public MainWindowViewModel(SmartPhoneMainViewModel smartPhoneMainViewModel,
                               SimCardMainViewModel simCardMainViewModel,
                               ServiceRequestMainViewModel serviceRequestMainViewModel,
                               SettingsMainViewModel settingsMainViewModel)
    {
        _smartPhoneMainViewModel = smartPhoneMainViewModel;
        _simCardMainViewModel = simCardMainViewModel;
        _serviceRequestMainViewModel = serviceRequestMainViewModel;
        _settingsMainViewModel = settingsMainViewModel;
    }

    public string ViewPackIcon => throw new NotImplementedException();

    public string ViewName => throw new NotImplementedException();

    [RelayCommand]
    private async Task UpdateViewAsync(object selectedViewModel)
    {
        if (selectedViewModel is null) return;

        if (selectedViewModel.GetType() != typeof(ViewType)) return;

        var viewType = (ViewType)selectedViewModel;
        switch (viewType)
        {
            case ViewType.Dashboard:
                throw new NotImplementedException();
            case ViewType.Phone:
                SelectedViewModel = _smartPhoneMainViewModel;
                break;
            case ViewType.SimCard:
                SelectedViewModel = _simCardMainViewModel;
                break;
            case ViewType.ServiceRequest:
                SelectedViewModel = _serviceRequestMainViewModel;
                break;
            case ViewType.Settings:
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