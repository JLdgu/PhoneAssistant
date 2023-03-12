using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using PhoneAssistant.WPF.Models;

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

    private void PhonesGrid_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        var dependencyObject = (DependencyObject)e.OriginalSource;

        //get clicked row from Visual Tree
        while ((dependencyObject != null) && !(dependencyObject is DataGridRow))
        {
            dependencyObject = VisualTreeHelper.GetParent(dependencyObject);
        }

        var row = dependencyObject as DataGridRow;
        if (row == null)
        {
            return;
        }

        row.IsSelected = true;        
    }

    private void PhonesGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        SelectedIMEI.Focus();
    }

    private void CollectionViewSource_Filter(object sender, System.Windows.Data.FilterEventArgs e)
    {
        Phone p = e.Item as Phone;
        if (p is null)
            return;

        var vm = (IPhonesMainViewModel)DataContext;


        // If filter is turned on, filter completed items.
        {
            if (p.Wiped == true)
                e.Accepted = false;
            else
                e.Accepted = true;
        }
    }
}
