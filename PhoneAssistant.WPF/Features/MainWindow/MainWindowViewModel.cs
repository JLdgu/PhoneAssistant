using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PhoneAssistant.WPF.Features.SmartPhone;
using PhoneAssistant.WPF.Features.SimCard;
using PhoneAssistant.WPF.Models;
using PhoneAssistant.WPF.Shared;
using System;
using System.Threading.Tasks;
using PhoneAssistant.WPF.Features.ServiceRequest;

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
    private readonly PhoneRepository _phoneRepository;
    private readonly SimRepository _simCardRepository;
    private readonly StateRepository _stateRepository;


    [ObservableProperty]
    private IViewModel? _selectedViewModel;

    public MainWindowViewModel(PhoneRepository phoneRepository, 
                               SimRepository simCardRepository, 
                               StateRepository stateRepository)
    {
        _phoneRepository = phoneRepository;
        _simCardRepository = simCardRepository;
        _stateRepository = stateRepository;
    }

    public string ViewPackIcon => throw new NotImplementedException();

    public string ViewName => throw new NotImplementedException();

    [RelayCommand]
    private async Task UpdateViewAsync(object selectedViewModel)
    {
        if (selectedViewModel is null) return;

        if (selectedViewModel.GetType() != typeof(ViewType))  return;
        
        var viewType = (ViewType) selectedViewModel;
        switch (viewType)
        {
            case ViewType.Phone:
                SelectedViewModel = new SmartPhoneMainViewModel(_phoneRepository, _stateRepository);
                break;
            case ViewType.SimCard:
                SelectedViewModel = new SimCardMainViewModel(_simCardRepository, _stateRepository);
                break;
            case ViewType.ServiceRequest:
                SelectedViewModel = new ServiceRequestMainViewModel();
                break;
        }
        await LoadAsync();
    }

    public async Task LoadAsync()
    {
        await SelectedViewModel!.LoadAsync();
    }
}