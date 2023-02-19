using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PhoneAssistant.WPF.Features.Phone;
using PhoneAssistant.WPF.Features.SimCard;
using PhoneAssistant.WPF.Models;
using PhoneAssistant.WPF.ViewModels;
using System.Threading.Tasks;

namespace PhoneAssistant.WPF.Features.MainWindow;

public sealed partial class MainWindowViewModel : ObservableObject, IViewModel
{
    private readonly PhoneRepository _phoneRepository;

    public MainWindowViewModel(PhoneRepository phoneRepository)
    {
        _phoneRepository = phoneRepository;
    }

    [ObservableProperty]
    private IViewModel? _selectedViewModel;

    [RelayCommand]
    private async Task UpdateViewAsync(string selectedViewModel)
    {
        if (selectedViewModel == "Phone")
        {
            SelectedViewModel = new PhoneMainViewModel(_phoneRepository);
        }
        else if (selectedViewModel.ToString() == "SIM")
        {
            SelectedViewModel = new SimMainViewModel();
        }
        await LoadAsync();
    }

    public async Task LoadAsync()
    {
        await SelectedViewModel!.LoadAsync();
    }

    //public MainViewModel()
    //{
    //}
}