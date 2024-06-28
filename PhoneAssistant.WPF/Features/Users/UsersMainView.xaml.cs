using System.Windows;
using System.Windows.Controls;

namespace PhoneAssistant.WPF.Features.Users;
/// <summary>
/// Interaction logic for UsersMainView.xaml
/// </summary>
public partial class UsersMainView : UserControl
{
    public UsersMainView()
    {
        InitializeComponent();
        DataObject.AddPastingHandler(Search, OnPaste);
    }

    private void OnPaste(object sender, DataObjectPastingEventArgs e)
    {
        UsersMainViewModel vm = (UsersMainViewModel)DataContext;
        bool isText = e.SourceDataObject.GetDataPresent(DataFormats.UnicodeText, true);
        if (!isText) return;
        Search.Text = e.SourceDataObject.GetData(DataFormats.UnicodeText) as string;
        vm.EnterKeyCommand.Execute(null);
        e.CancelCommand();
    }
}
