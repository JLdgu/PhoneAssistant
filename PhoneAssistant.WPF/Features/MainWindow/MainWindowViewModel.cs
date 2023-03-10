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
    None,
    Dashboard,
    Phones,
    Sims,
    ServiceRequests,
    Settings
}

public sealed partial class MainWindowViewModel : ObservableObject
{
    private readonly IPhonesMainViewModel _phonesMainViewModel;
    private readonly ISimsMainViewModel _simCardMainViewModel;
    private readonly IServiceRequestsMainViewModel _serviceRequestMainViewModel;
    private readonly ISettingsMainViewModel _settingsMainViewModel;

    [ObservableProperty]
    private IViewModel? _selectedViewModel;

    public MainWindowViewModel(IPhonesMainViewModel phonesMainViewModel,
                               ISimsMainViewModel simCardMainViewModel,
                               IServiceRequestsMainViewModel serviceRequestMainViewModel,
                               ISettingsMainViewModel settingsMainViewModel)
    {
        _phonesMainViewModel = phonesMainViewModel;
        _simCardMainViewModel = simCardMainViewModel;
        _serviceRequestMainViewModel = serviceRequestMainViewModel;
        _settingsMainViewModel = settingsMainViewModel;
    }

    [RelayCommand]
    private async Task UpdateViewAsync(object selectedViewModelType)
    {
        if (selectedViewModelType is null) 
            throw new ArgumentNullException(nameof(selectedViewModelType));

        if (selectedViewModelType.GetType() != typeof(ViewModelType))
            throw new ArgumentException("Type " + selectedViewModelType.GetType() + " is not handled."); 
        
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
        await SelectedViewModel.LoadAsync();
    }
}