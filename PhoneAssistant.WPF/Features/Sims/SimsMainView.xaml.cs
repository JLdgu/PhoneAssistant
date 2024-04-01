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
        MaxHeight = System.Windows.SystemParameters.VirtualScreenHeight;
    }
    private void StackPanel_GotKeyboardFocus(object sender, System.Windows.Input.KeyboardFocusChangedEventArgs e)
    {
        SimsDataGrid.CommitEdit(DataGridEditingUnit.Row, true);
    }
}
