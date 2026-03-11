using System.ComponentModel;
using System.Windows.Controls;

namespace PhoneAssistant.WPF.Features.Phones;

public partial class EmailView : UserControl
{
    public EmailView()
    {
        InitializeComponent();

        // Don't run the WebView2 initialization when the XAML designer is active
        if (DesignerProperties.GetIsInDesignMode(this))
            return;

        _ = InitializeAsync();
    }
    async Task InitializeAsync()
    {
        try
        {
            if (WebView2 is null)
                return;

            await WebView2.EnsureCoreWebView2Async(null);
            WebView2.CoreWebView2?.Profile.PreferredColorScheme = Microsoft.Web.WebView2.Core.CoreWebView2PreferredColorScheme.Light;
            WebView2.DefaultBackgroundColor = System.Drawing.Color.LightGray;
        }
        catch (Exception)
        {
            // Swallow exceptions during initialization to avoid crashing the XAML designer
        }
    }
    private void DeliveryAddress_TextChanged(object sender, TextChangedEventArgs e)
    {
        string newValue = EmailViewModel.ReformatDeliveryAddress(DeliveryAddress.Text);
        if (newValue != DeliveryAddress.Text)
            DeliveryAddress.Text = newValue;
    }
}