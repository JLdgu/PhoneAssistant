using System.Windows.Controls;

namespace PhoneAssistant.WPF.Features.BaseReport;
/// <summary>
/// Interaction logic for BaseReportMainView.xaml
/// </summary>
public partial class BaseReportMainView : UserControl
{
    public BaseReportMainView()
    {
        InitializeComponent();        
    }

    private void StackPanel_GotKeyboardFocus(object sender, System.Windows.Input.KeyboardFocusChangedEventArgs e)
    {
        BaseReportDataGrid.CommitEdit(DataGridEditingUnit.Row, true);
    }
}
