using System.Windows;

using PhoneAssistant.WPF.Features.MainWindow;

namespace PhoneAssistant.WPF;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow(MainWindowViewModel viewModel)
    {
        InitializeComponent();        

        DataContext = viewModel;
    }
}
