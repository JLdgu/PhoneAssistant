using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace PhoneAssistant.WPF.Features.Settings;
/// <summary>
/// Interaction logic for SettingsMainView.xaml
/// </summary>
public partial class SettingsMainView : UserControl
{
    public SettingsMainView()
    {
        InitializeComponent();
    }
    private void ReleaseNotes_Click(object sender, RoutedEventArgs e)
    {
        Process.Start(new ProcessStartInfo
        {
            FileName = "https://github.com/JLdgu/PhoneAssistant/releases",
            UseShellExecute = true // Ensures it opens in the default browser
        });
    }
}
