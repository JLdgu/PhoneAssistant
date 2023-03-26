using System.Windows.Controls;

using PhoneAssistant.WPF.Shared;

namespace PhoneAssistant.WPF.Features.Sims;
/// <summary>
/// Interaction logic for SimsMainView.xaml
/// </summary>
public partial class SimsMainView : UserControl
{
    public SimsMainView()
    {
        InitializeComponent();
    }

    private void SimsGrid_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        => UI_Interactions.SelectRowFromWhiteSpaceClick(e);
}
