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
        DataObject.AddPastingHandler(SearchName, OnPaste);
        DataObject.AddPastingHandler(SearchEmail, OnPaste);
    }

    private void OnPaste(object sender, DataObjectPastingEventArgs e)
    {
        bool isText = e.SourceDataObject.GetDataPresent(DataFormats.UnicodeText, true);
        if (!isText) return;
        if (sender is not TextBox textBox)
            return;

        UsersMainViewModel vm = (UsersMainViewModel)DataContext;
        if (textBox.Name == "SearchName")
        {
            SearchName.Text = e.SourceDataObject.GetData(DataFormats.UnicodeText) as string;
            vm.SearchNameCommand.Execute(null);
        }
        else
        {
            SearchEmail.Text = e.SourceDataObject.GetData(DataFormats.UnicodeText) as string;
            vm.SearchEmailCommand.Execute(null);
        }
        e.CancelCommand();
    }
}
