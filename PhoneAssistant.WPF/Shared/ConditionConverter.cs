using System.Globalization;
using System.Windows.Data;

namespace PhoneAssistant.WPF.Shared;

[ValueConversion(typeof(string), typeof(string))]
public class ConditionConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        string? nOrR = value as string;
        nOrR = nOrR == "N" ? "New" : "Repurposed";

        return nOrR;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        string? nOrR = value as string;
        if (nOrR is null)
            return "N";

        if (nOrR.StartsWith("N", StringComparison.InvariantCultureIgnoreCase))
            nOrR = "N";
        else
            nOrR = "R";

        return nOrR;
    }
}
