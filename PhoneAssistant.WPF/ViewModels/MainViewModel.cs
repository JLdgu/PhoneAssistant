using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace PhoneAssistant.WPF.ViewModels;

public partial class MainViewModel : ObservableObject, IMainViewModel
{
    [ObservableProperty]
    private IMainViewModel? _selectedViewModel;

    [RelayCommand]
    private void UpdateView(string selectedViewModel)
    {
        if (selectedViewModel == "Phone")
        {
            SelectedViewModel = new PhoneMainViewModel();
        }
        else if (selectedViewModel.ToString() == "SIM")
        {
            SelectedViewModel = new SimMainViewModel();
        }

    }

    public MainViewModel()
    {
    }
}