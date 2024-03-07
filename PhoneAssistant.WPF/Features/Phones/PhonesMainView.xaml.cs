using System.Windows.Controls;

using PhoneAssistant.WPF.Shared;

namespace PhoneAssistant.WPF.Features.Phones;
/// <summary>
/// Interaction logic for PhonesMainView.xaml
/// </summary>
public partial class PhonesMainView : UserControl
{
    public PhonesMainView()
    {
        InitializeComponent();
    }

    private void StackPanel_GotKeyboardFocus(object sender, System.Windows.Input.KeyboardFocusChangedEventArgs e)
    {
        PhonesDataGrid.CommitEdit(DataGridEditingUnit.Row, true);
    }
}
