using System.Windows;
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
        DataObject.AddPastingHandler(Search, OnPaste);
    }

    private void OnPaste(object sender, DataObjectPastingEventArgs e)
    {
        BaseReportMainViewModel vm = (BaseReportMainViewModel)DataContext;
        
        bool isText = e.SourceDataObject.GetDataPresent(DataFormats.UnicodeText, true);
        if (!isText) return;

        Search.Text = e.SourceDataObject.GetData(DataFormats.UnicodeText) as string;
        vm.EnterKeyCommand.Execute(null);
        e.CancelCommand();
    }
}
