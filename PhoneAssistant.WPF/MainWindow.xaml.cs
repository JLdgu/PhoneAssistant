using PhoneAssistant.WPF.Models;
using PhoneAssistant.WPF.ViewModels;
using System.Windows;

namespace PhoneAssistant.WPF;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{    
    public MainWindow(MainViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
