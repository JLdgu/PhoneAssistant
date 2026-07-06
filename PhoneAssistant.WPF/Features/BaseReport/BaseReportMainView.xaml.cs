using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PhoneAssistant.WPF.Features.BaseReport;
/// <summary>
/// Interaction logic for BaseReportMainView.xaml
/// </summary>
public partial class BaseReportMainView : UserControl
{
    public BaseReportMainView()
    {
        InitializeComponent();
        DataObject.AddPastingHandler(SearchPhoneNumber, OnPastePhoneNumber);
        DataObject.AddPastingHandler(SearchSimNumber, OnPasteSimNumber);
        DataObject.AddPastingHandler(SearchUserName, OnPasteUserName);
    }

    private static void HandlePaste(object sender, DataObjectPastingEventArgs e, string expectedTextBoxName, ICommand searchCommand)
    {
        if (sender is not TextBox textBox ||
            textBox.Name != expectedTextBoxName ||
            !e.SourceDataObject.GetDataPresent(DataFormats.UnicodeText, true))
        {
            return;
        }

        textBox.Text = e.SourceDataObject.GetData(DataFormats.UnicodeText) as string;

        searchCommand.Execute(null);
        e.CancelCommand();
    }

    private void OnPastePhoneNumber(object sender, DataObjectPastingEventArgs e)
    {
        var vm = (BaseReportMainViewModel)DataContext;
        HandlePaste(sender, e, "SearchPhoneNumber", vm.PhoneNumberSearchCommand);
    }

    private void OnPasteSimNumber(object sender, DataObjectPastingEventArgs e)
    {
        var vm = (BaseReportMainViewModel)DataContext;
        HandlePaste(sender, e, "SearchSimNumber", vm.SimNumberSearchCommand);
    }

    private void OnPasteUserName(object sender, DataObjectPastingEventArgs e)
    {
        var vm = (BaseReportMainViewModel)DataContext;
        HandlePaste(sender, e, "SearchUserName", vm.UserNameSearchCommand);
    }

}
