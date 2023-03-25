using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows;

namespace PhoneAssistant.WPF.Shared
{
    public static class UI_Interactions
    {
        public static void SelectRowFromWhiteSpaceClick(MouseButtonEventArgs e)
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
    }
}
