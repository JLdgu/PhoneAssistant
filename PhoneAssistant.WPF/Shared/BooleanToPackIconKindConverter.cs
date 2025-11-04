using MaterialDesignThemes.Wpf;
using System.Globalization;
using System.Windows.Data;

namespace PhoneAssistant.WPF.Shared
{
    public sealed class BooleanToPackIconKindConverter : IValueConverter
    {
        // Returns Email when true, EmailOpen when false
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool flag = value is bool b && b;
            return flag ? PackIconKind.EmailOutline : PackIconKind.EmailOpenOutline;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotSupportedException();
    }
}
