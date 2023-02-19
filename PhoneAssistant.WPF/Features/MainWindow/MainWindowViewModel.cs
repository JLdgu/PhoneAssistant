using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PhoneAssistant.WPF.Features.Phone;
using PhoneAssistant.WPF.Features.SimCard;
using PhoneAssistant.WPF.Models;
using PhoneAssistant.WPF.Shared;
using System;
using System.Threading.Tasks;

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

    public MainWindowViewModel(PhoneRepository phoneRepository)
    {
        _phoneRepository = phoneRepository;
    }

    [ObservableProperty]
    private IViewModel? _selectedViewModel;

    public string ViewPackIcon => throw new NotImplementedException();

    public string ViewName => throw new NotImplementedException();

    [RelayCommand]
    private async Task UpdateViewAsync(object selectedViewModel)
    {
        if (selectedViewModel is null) return;

        if (selectedViewModel.GetType() != typeof(ViewType))  return;
        var viewType = (ViewType) selectedViewModel;

        if (viewType == ViewType.Phone)
        {
            SelectedViewModel = new PhoneMainViewModel(_phoneRepository);
        }
        else if (viewType == ViewType.SimCard)
        {
            SelectedViewModel = new SimMainViewModel();
        }
        await LoadAsync();
    }

    public async Task LoadAsync()
    {
        await SelectedViewModel!.LoadAsync();
    }
}