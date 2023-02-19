using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PhoneAssistant.WPF.Features.Phone;
using PhoneAssistant.WPF.Features.SimCard;
using PhoneAssistant.WPF.Models;
using System.Threading.Tasks;

namespace PhoneAssistant.WPF.ViewModels;

public sealed partial class MainViewModel : ObservableObject, IMainViewModel
{
    private readonly PhoneRepository _phoneRepository;

    public MainViewModel(PhoneRepository phoneRepository)
    {
        _phoneRepository = phoneRepository;
    }

    [ObservableProperty]
    private IMainViewModel? _selectedViewModel;

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