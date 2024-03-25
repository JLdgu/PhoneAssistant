using System.Windows;
using System.Windows.Controls;

namespace PhoneAssistant.WPF.Features.Phones;

public static class WebBrowserHelper
{
    public static readonly DependencyProperty NavigateToProperty =
        DependencyProperty.RegisterAttached("NavigateTo", typeof(string), typeof(WebBrowserHelper), new UIPropertyMetadata(null, NavigateToPropertyChanged));

    public static string GetNavigateTo(DependencyObject obj)
    {
        return (string)obj.GetValue(NavigateToProperty);
    }

    public static void SetNavigateTo(DependencyObject obj, string value)
    {
        obj.SetValue(NavigateToProperty, value);
    }

    public static void NavigateToPropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
    {
        if (o is WebBrowser browser)
            if (e.NewValue is string html)
                if (!string.IsNullOrEmpty(html))
                    browser.NavigateToString(html);
    }
}
