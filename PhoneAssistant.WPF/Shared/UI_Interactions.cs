using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows;
using System.Runtime.Versioning;

namespace PhoneAssistant.WPF.Shared
{
    public static class UI_Interactions
    {
        public static void SelectRowFromWhiteSpaceClick(MouseButtonEventArgs e)
        {
            var dependencyObject = (DependencyObject)e.OriginalSource;

            //get clicked row from Visual Tree
            while ((dependencyObject != null) && (dependencyObject is not DataGridRow))
            {
                dependencyObject = VisualTreeHelper.GetParent(dependencyObject);
            }

            
            if (dependencyObject is not DataGridRow row)
            {
                return;
            }

            row.IsSelected = true;
        }
    }
}
